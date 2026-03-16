namespace GodotLib.Util;

using Godot;
using System;

public static class TreeExtensions
{
    /// <summary>
    /// Tries to get a child, and if it doesn't exist, creates a new one
    /// </summary>
    /// <returns>true if child is new, false if it already existed</returns>
    public static bool CreateOrGetChild(this TreeItem parentItem, int childIndex, out TreeItem child)
    {
        if (parentItem.GetChildCount() > childIndex)
        {
            child = parentItem.GetChild(childIndex);
            return false;
        }

        child = parentItem.CreateChild();
        return true;
    }

    public static TreeItem CreateChildAlphabetically(this TreeItem parentItem, string text, int textColumn = 0)
    {
        var insertIndex = GetInsertIndex(parentItem, text, textColumn);
        var child = parentItem.CreateChild(insertIndex);
        child.SetText(textColumn, text);

        return child;
    }

    private static int GetInsertIndex(TreeItem parent, string newText, int column)
    {
        var child = parent.GetFirstChild();
        var index = 0;

        while (child != null)
        {
            if (string.Compare(newText, child.GetText(column), StringComparison.OrdinalIgnoreCase) < 0)
                return index;

            index++;
            child = child.GetNext();
        }

        return index;
    }
    
    public static TreeItem CreateChildById(this TreeItem parentItem, int id, int column = 0)
    {
        var insertIndex = GetInsertIndex(parentItem, id, column);
        var child = parentItem.CreateChild(insertIndex);
        child.SetMetadata(column, id);
        return child;
    }
    
    public static TreeItem CreateChildById(this TreeItem parentItem, uint id, int column = 0)
    {
        var insertIndex = GetInsertIndex(parentItem, id, column);
        var child = parentItem.CreateChild(insertIndex);
        child.SetMetadata(column, id);
        return child;
    }
    
    private static int GetInsertIndex(TreeItem parent, int id, int column)
    {
        var child = parent.GetFirstChild();
        var index = 0;

        while (child != null)
        {
            var otherId = child.GetMetadata(column).AsInt32();
            if (id.CompareTo(otherId) < 0)
                return index;

            index++;
            child = child.GetNext();
        }

        return index;
    }
    
    private static int GetInsertIndex(TreeItem parent, uint id, int column)
    {
        var child = parent.GetFirstChild();
        var index = 0;

        while (child != null)
        {
            var otherId = child.GetMetadata(column).AsUInt32();
            if (id.CompareTo(otherId) < 0)
                return index;

            index++;
            child = child.GetNext();
        }

        return index;
    }

    public static TreeItem CreateChildWithMetadataOrdering(this TreeItem parentItem, Func<Variant, Variant, int> comparisonFunc, int comparisonColumn, Variant metadata)
    {
        var insertIndex = GetInsertIndex(parentItem, comparisonFunc, comparisonColumn, metadata);
        var child = parentItem.CreateChild(insertIndex);
        child.SetMetadata(comparisonColumn, metadata);
        return child;
    }

    private static int GetInsertIndex(TreeItem parent, Func<Variant, Variant, int> comparisonFunc, int column, Variant newMetadata)
    {
        var child = parent.GetFirstChild();
        var index = 0;

        while (child != null)
        {
            var childMetadata = child.GetMetadata(column);

            if (comparisonFunc(newMetadata, childMetadata) > 0)
                return index;

            index++;
            child = child.GetNext();
        }

        return index;
    }
}
