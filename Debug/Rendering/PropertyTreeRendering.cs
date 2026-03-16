using System.Collections;
using System.Text.RegularExpressions;
using GodotLib.Util;
using System.Collections.Generic;
using System.Reflection;
using AstorGames.EcsTools;
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

    public static TreeItem Render(TreeItem parentItem, int childIndex, object component, string fieldName, RenderingParameters parameters)
    {
        var componentType = component?.GetType();
        if (componentType?.GetCustomAttribute<InspectorHiddenAttribute>() != null || parameters.Depth > 3)
        {
            return parentItem;
        }

        parameters.Depth++;
        parameters.IsNew = parentItem.CreateOrGetChild(childIndex, out var componentItem);

        fieldName = BakingFieldRegex().Replace(fieldName, "${name}");
        if (parameters.HumanizeName)
        {
            fieldName = StringUtils.HumanizeName(fieldName);
        }
        
        if (!string.IsNullOrEmpty(fieldName))
            componentItem.SetText(0, fieldName);

        if (parameters.Highlighted)
        {
            componentItem.SetCustomBgColor(0, selectedColor);
            componentItem.SetExpandRight(0, true);
        }

        parameters.Highlighted = false;

        if (parameters.IsNew)
        {
            componentItem.Collapsed |= parameters.StartCollapsed;
        }

        if (component == null)
        {
            componentItem.SetText(1, "null");
            componentItem.SetCustomColor(1, RendererConsts.ErrorColor);
            return componentItem;
        }
        
        componentItem.SetTooltipText(0, $"Type: {componentType.GetHumanReadableName()}");

        if (componentType.IsPrimitive || componentType.IsEnum || componentType == typeof(string))
        {
            // Special handling for flag enums showing 0
            if (componentType.IsEnum)
            {
                if (componentType.GetCustomAttribute<FlagsAttribute>() != null && Convert.ToInt64(component) == 0)
                {
                    componentItem.SetText(1, "None");
                    componentItem.SetCustomColor(1, RendererConsts.DefaultValueColor);
                    return componentItem;
                }

                if (parameters.HumanizeName)
                {
                    componentItem.SetText(1, StringUtils.HumanizeName(component.ToString()));
                    return componentItem;
                }
            }

            componentItem.SetText(1, component.ToString());
            return componentItem;
        }

        if (customRenderers.TryGetValue(componentType, out var customRenderer))
        {
            customRenderer.Render(componentItem, component, parameters);
            return componentItem;
        }
        
        if (componentType.IsGenericType)
        {
            var genericTypeDefinition = componentType.GetGenericTypeDefinition();
            if (genericTypedRenderers.TryGetValue(genericTypeDefinition, out var rendererType))
            {
                if (rendererType.IsGenericTypeDefinition)
                {
                    customRenderer = (IPropertyTreeRenderer)Activator.CreateInstance(rendererType.MakeGenericType(componentType.GetGenericArguments()));
                }
                else
                {
                    customRenderer = (IPropertyTreeRenderer)Activator.CreateInstance(rendererType);
                }
              
                customRenderer!.Render(componentItem, component, parameters);

                // Cache the instance in customRenderers for reuse
                customRenderers[componentType] = customRenderer;
                return componentItem;
            }
        }
        
        if (componentType.IsAssignableTo(typeof(IList)))
        {
            listRenderer.Render(componentItem, component, parameters);
            return componentItem;
        }

        if (stringRenderedTypes.Contains(componentType))
        {
            componentItem.SetText(1, component.ToString());
            return componentItem;
        }

        RenderFields(componentItem, component, parameters);
        return componentItem;
    }

    public static void RenderFields(TreeItem parentItem, object component, RenderingParameters parameters)
    {
        var componentType = component.GetType();
        var fields = componentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var fieldIndex = 0;
        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<InspectorHiddenAttribute>() != null)
            {
                return;
            }
            
            try
            {
                var fieldValue = field.GetValue(component);
                Render(parentItem, fieldIndex++, fieldValue, field.Name, parameters);
            }
            catch (Exception e)
            {
                parentItem.CreateOrGetChild(fieldIndex, out var child);
                child.SetText(0, $"{field.Name} | {field.FieldType}: ERROR");
                child.SetTooltipText(0, e.Message);
                child.SetCustomColor(0, RendererConsts.ErrorColor);
                return;
            }
        }
    }
}
