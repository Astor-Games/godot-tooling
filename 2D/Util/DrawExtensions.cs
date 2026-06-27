namespace GodotLib;

public static class DrawExtensions
{
    extension(Node2D node2D)
    {
        public void DrawGrid(int rows, int columns, Vector2 origin, float scale, Color color, float width, bool antialiased = false)
        {
            var pointCount = (rows + 1) * 2 + (columns + 1) * 2;
            Span<Vector2> points = stackalloc Vector2[pointCount];

            var pointIdx = 0;
        
            for (var x = 0; x <= rows; x++)
            {
                points[pointIdx++] = origin + new Vector2(x - 0.5f, -0.5f) * scale;
                points[pointIdx++] = origin + new Vector2(x - 0.5f, columns-0.5f) * scale;
            }
        
            for (var y = 0; y <= columns; y++)
            {
                points[pointIdx++] = origin +new Vector2(-0.5f, y - 0.5f) * scale;
                points[pointIdx++] = origin + new Vector2(rows - 0.5f, y - 0.5f) * scale;
            }
        
            node2D.DrawMultiline(points, color, width, antialiased);
        }
    }
}