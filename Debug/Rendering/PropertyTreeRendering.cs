using System.Collections;
using System.Text.RegularExpressions;
using GodotLib.Util;
using System.Collections.Generic;
using System.Reflection;
using Collections.Pooled;
using Convert = System.Convert;

namespace GodotLib.Debug;

public static partial class PropertyTreeRendering
{
    [GeneratedRegex(@"<(?<name>\w+)>k__BackingField")]
    private static partial Regex BakingFieldRegex();

    private static readonly Dictionary<Type, Type> genericTypedRenderers = new();
    private static readonly Dictionary<Type, IPropertyTreeRenderer> customRenderers = new();
    private static readonly HashSet<Type> stringRenderedTypes = new();
    private static readonly ListRenderer listRenderer = new();

    private static readonly Color selectedColor = new Color(1, 1, 1, 0.1f);

    static PropertyTreeRendering()
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (typeof(IPropertyTreeRenderer).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
            {
                var attr = (PropertyRendererAttribute)Attribute.GetCustomAttribute(type, typeof(PropertyRendererAttribute));
                if (attr == null) continue;

                foreach (var renderedType in attr.RenderedTypes)
                {
                    if (renderedType.IsArray || renderedType.IsGenericTypeDefinition)
                    {
                        genericTypedRenderers[renderedType] = type;
                        //GD.Print($"Loaded generic type {type.Name}");
                    }
                    else
                    {
                        var instance = Activator.CreateInstance(type) as IPropertyTreeRenderer;
                        customRenderers[renderedType] = instance;
                        //GD.Print($"Loaded type {type.Name}");
                    }
                }
            }
        }
    }

    public static void RenderTypeAsString<T>()
    {
        stringRenderedTypes.Add(typeof(T));
    }

    public static void Render(TreeItem parentItem, ReadOnlySpan<string> names, ReadOnlySpan<object> values, RenderingParameters parameters)
    {
        if (parameters.Depth > 3) return;
        
        var count = names.Length;

        using var indices = new PooledSet<int>();

        for (var i = 0; i < count; i++)
        {
            try
            {
                var value = values[i];
                var name = names[i];
                var propertyType = value?.GetType();
                if (propertyType?.GetCustomAttribute<InspectorHiddenAttribute>() != null) continue;
               
                var index = RenderSingleProperty(parentItem, i, name, value, parameters);
                indices.Add(index); 
            }
            catch (Exception e)
            {
                var child = parentItem.GetChild(parentItem.GetChildCount() -1);
                child.SetText(0, $"{names[i]}: ERROR");
                child.SetTooltipText(0, e.Message);
                child.SetCustomColor(0, RendererConsts.ErrorColor);
                
                return;
            }
        }
        
        var allChildren = parentItem.GetChildCount();
        for (var i = 0; i < allChildren; i++)
        {
            if (!indices.Contains(i))
            {
                var child = parentItem.GetChild(i);
                parentItem.RemoveChild(child);
            }
        }
    }

    private static int RenderSingleProperty(TreeItem parentItem, int index, string name, object value, RenderingParameters parameters)
    {
        parameters.Depth++;
        parameters.IsNew = GetOrCreateItem(parentItem, name.GetHashCode(), out var item, ref index);
        
        var displayName = BakingFieldRegex().Replace(name, "${name}");
        if (parameters.HumanizeName)
        {
            displayName = StringUtils.HumanizeName(name);
        }
        
        item.SetText(0, displayName);

        if (parameters.Highlighted)
        {
            item.SetCustomBgColor(0, selectedColor);
            item.SetExpandRight(0, true);
        }

        parameters.Highlighted = false;

        if (parameters.IsNew)
        {
            GD.Print("new!");
            item.Collapsed |= parameters.StartCollapsed;
        }

        if (value == null)
        {
            item.SetText(1, "null");
            item.SetCustomColor(1, RendererConsts.ErrorColor);
            return index;
        }

        var type = value.GetType();
        item.SetTooltipText(0, $"Type: {type.GetHumanReadableName()}");

        if (type.IsPrimitive || type.IsEnum || type == typeof(string))
        {
            // Special handling for flag enums showing 0
            if (type.IsEnum)
            {
                if (type.GetCustomAttribute<FlagsAttribute>() != null && Convert.ToInt64(value) == 0)
                {
                    item.SetText(1, "None");
                    item.SetCustomColor(1, RendererConsts.DefaultValueColor);
                    return index;
                }

                if (parameters.HumanizeName)
                {
                    item.SetText(1, StringUtils.HumanizeName(value.ToString()));
                    return index;
                }
            }

            item.SetText(1, value.ToString());
            return index;
        }

        if (customRenderers.TryGetValue(type, out var customRenderer))
        {
            customRenderer.Render(item, value, parameters);
            return index;
        }

        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypedRenderers.TryGetValue(genericTypeDefinition, out var rendererType))
            {
                if (rendererType.IsGenericTypeDefinition)
                {
                    customRenderer = (IPropertyTreeRenderer)Activator.CreateInstance(rendererType.MakeGenericType(type.GetGenericArguments()));
                }
                else
                {
                    customRenderer = (IPropertyTreeRenderer)Activator.CreateInstance(rendererType);
                }

                customRenderer!.Render(item, value, parameters);

                // Cache the instance in customRenderers for reuse
                customRenderers[type] = customRenderer;
                return index;
            }
        }

        if (type.IsAssignableTo(typeof(IList)))
        {
            listRenderer.Render(item, value, parameters);
            return index;
        }

        if (stringRenderedTypes.Contains(type))
        {
            item.SetText(1, value.ToString());
            return index;
        }

        RenderFields(item, value, parameters);
        return index;
    }

    private static bool GetOrCreateItem(TreeItem parentItem, int id, out TreeItem item, ref int index)
    {
        var items = parentItem.GetChildren();
        for (int i = 0; i < items.Count; i++)
        {
            var child = items[i];
            if (child.GetMeta("element_id", 0).AsInt32() == id)
            {
                item = child;
                index = i;
                return false;
            }
        }

        item = parentItem.CreateChild(index);
        item.SetMeta("element_id", id);
        return true;
    }

    private static void RenderFields(TreeItem parentItem, object component, RenderingParameters parameters)
    {
        var componentType = component.GetType();
        var fields = componentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        using var names = new PooledList<string>(fields.Length);
        using var values = new PooledList<object>(fields.Length);

        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<InspectorHiddenAttribute>() != null)
            {
                continue;
            }

            names.Add(field.Name);
            values.Add(field.GetValue(component));
        }

        Render(parentItem, names.Span, values.Span, parameters);
    }
}