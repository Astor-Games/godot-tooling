using System.Collections.Generic;

namespace GodotLib;

/// <summary>IHashable represents an object which
/// can be inserted into a Spatial Hash and queried
/// using a key (two cell indices)</summary>
public interface IHashable
{
    public (CellIndex start, CellIndex end) RegisteredHashBounds { get; set; }
    public int QueryId { get; set; }
}


/// <summary>CellIndex repesents a position as
/// coordinates on a grid</summary>
public struct CellIndex
{
    public int X;
    public int Y;

    public CellIndex(int x, int y)
    {
        X = x;
        Y = y; 
    }

    public static bool operator ==(CellIndex a, CellIndex b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(CellIndex a, CellIndex b)
    {
        return a.X != b.X || a.Y != b.Y;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is CellIndex))
        {
            return false;
        }

        return Equals((CellIndex)obj);
    }

    public bool Equals(CellIndex other)
    {
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return GenerateHashCode((int)X, (int)Y);
    }
    
    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    private static int GenerateHashCode(int x, int y)
    {
        return (x << 2) ^ y;
    }
    
}

/// <summary>A fast fixed-size Spatial Hash implementation</summary>
public class SpatialHash<T> where T : IHashable
{
    private readonly (CellIndex, CellIndex) InvalidBounds = (new CellIndex(-1,-1), new CellIndex(-1,-1));
    
    /// <summary>Multidimensional array as [x, y] to represent grid position</summary>
    public readonly List<T>[,] Grid;
    
    
    /// <summary>Unique identifier to deduplicate colliders
    /// that exist in multiple buckets in a single query</summary>
    private int _queryId;

    public readonly Rect2 Bounds;
    public readonly int Width;
    public readonly int Height;
    private readonly float invSizeX, invSizeY;

    public SpatialHash(Rect2 bounds, int width, int height)
    {
        Grid = new List<T>[width, height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                Grid[x, y] = [];
            }
        }

        Bounds = bounds.Abs();
        Width = width;
        Height = height;
        
        invSizeX = Width / Bounds.Size.X;
        invSizeY = Height / Bounds.Size.Y;
    }

    /// <summary>Returns a 2D position as an index within the grid space</summary>
    /// <param name="position">World position of hashed object</param>
    public CellIndex GetCellIndex(Vector2 position)
    {
        var x = (int)Math.Floor((position.X - Bounds.Position.X) * invSizeX);
        var y = (int)Math.Floor((position.Y - Bounds.Position.Y) * invSizeY);

        /***
         * Calculated index clamped between 0 and total # of cells
         * to prevent index going out of bounds
         */
        var clampedX = Math.Min(Width - 1, Math.Max(0, x));
        var clampedY = Math.Min(Height - 1, Math.Max(0, y));

        return new CellIndex(clampedX, clampedY);
    }

    /// <summary>Add a specified collider to every bucket on the grid between it's top left and bottom right bounds</summary>
    public void Insert(ref T collider, Vector2 topLeftBounds, Vector2 bottomRightBounds)
    {
        var startCoordinates = GetCellIndex(topLeftBounds);
        var endCoordinates = GetCellIndex(bottomRightBounds);

        collider.RegisteredHashBounds = (startCoordinates, endCoordinates);

        /***
         * Use top left and bottom right corners of collider bounds
         * to find every cell in between that the collider belongs in
         */
        for (int x0 = startCoordinates.X, x1 = endCoordinates.X; x0 <= x1; x0++)
        {
            for (int y0 = startCoordinates.Y, y1 = endCoordinates.Y; y0 <= y1; y0++)
            {
                Grid[x0, y0].Add(collider);
            }
        }
    }
    

    /// <summary>Remove a collider from every bucket it belongs to and nullify the key</summary>
    public void Remove(T collider)
    { 
        if (collider.RegisteredHashBounds != InvalidBounds)
        {
            /***
            * Need to explicitly coerce bounds as non-null tuple
            * because IHashable type is nullable
            */
            var (start, end) = collider.RegisteredHashBounds;

            collider.RegisteredHashBounds = InvalidBounds;

            for (int x0 = start.X, x1 = end.X; x0 <= x1; x0++)
            {
                for (int y0 = start.Y, y1 = end.Y; y0 <= y1; y0++)
                {
                    Grid[x0, y0].Remove(collider);
                }
            }
        }
    }

    /// <summary>Update a collider (and it's buckets) by removing and then re-inserting it</summary>
    public void UpdateCollider(ref T collider, Vector2 topLeftBounds, Vector2 bottomRightBounds)
    {
        /***
         * Do not need to update hashed collider if bounds have not moved enough
         * to change cells
         */
        if (ColliderHasMovedCells(collider, topLeftBounds, bottomRightBounds))
        {
            Remove(collider);
            Insert(ref collider, topLeftBounds, bottomRightBounds);
        }
    }

    /// <summary>Returns whether or not a collider has moved enough to change cells</summary>
    public bool ColliderHasMovedCells(IHashable collider, Vector2 topLeftBounds, Vector2 bottomRightBounds)
    {
        var startCoordinates = GetCellIndex(topLeftBounds);
        var endCoordinates = GetCellIndex(bottomRightBounds);

        return collider.RegisteredHashBounds != (startCoordinates, endCoordinates);
    }

    /// <summary>Returns all colliders an entity shares a bucket with (no repeats and self not returned)</summary>
    /// <param name="collider">Target collider</param>
    /// <param name="radius">Amount of additional cells to check in every direction</param>
    public IEnumerable<T> FindNearbyColliders((CellIndex, CellIndex) hashBounds, int radius = 0)
    {
        var metrics = (0, 0);
        
        if (hashBounds != InvalidBounds)
        {
            var (start, end) = hashBounds;

            var startX = Math.Max(0, start.X - radius);
            var startY = Math.Max(0, start.Y - radius);

            var endX = Math.Min(Width - 1, end.X + radius);
            var endY = Math.Min(Height - 1, end.Y + radius);

            /***
            * Iterate to ensure unique query id
            */
            var queryId = _queryId++;

            for (int x0 = startX, x1 = endX; x0 <= x1; x0++)
            {
                for (int y0 = startY, y1 = endY; y0 <= y1; y0++)
                {
                    //metrics.cells++;
                    foreach (var coll in Grid[x0, y0])
                    {
                        if (coll.QueryId != queryId)
                        {
                            /***
                            * Set collider query id to current query id to prevent
                            * duplicate object from same query in nearby bucket
                            */
                            //metrics.colliders++;
                            coll.QueryId = queryId;
                            yield return coll;
                        }
                    }
                }
            }
        }
    }

    public void Clear()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                Grid[x, y].Clear();
            }
        }
    }
}