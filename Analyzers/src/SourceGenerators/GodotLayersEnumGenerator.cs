#define DEBUG

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GodotLib.Analyzers.SourceGenerators.Utils;
using Microsoft.CodeAnalysis;

namespace GodotLib.Analyzers.SourceGenerators;

[Generator]
public class GodotLayerEnumGenerator : ISourceGenerator
{
    private const bool GenerateUnnamedValues = false;

    private readonly Dictionary<string, (string name, int maxValue)> layerTypes = new()
    {
        { "2d_physics", ("PhysicsLayers2D", 32) },
        { "3d_physics", ("PhysicsLayers3D", 32) },
        { "2d_navigation", ("NavigationLayers2D", 32) },
        { "3d_navigation", ("NavigationLayers3D", 32) },
        { "2d_render", ("RenderLayers2D", 20) },
        { "3d_render", ("RenderLayers3D", 20) }
    };

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization needed for ISourceGenerator
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var projectText = GodotProjectHelper.ReadGodotProject(context);
        var layersData = ExtractLayerNames(projectText);
       
        foreach (var (godotName, className, maxValue) in layerTypes)
        {
            GenerateLayersFile(context, godotName, className, maxValue, layersData);
        }
    }

    private void GenerateLayersFile(GeneratorExecutionContext context, string godotName, string cSharpName, int maxValue, Dictionary<string, string> layerData)
    {
        var enumMembers = new List<(string name, int value)>();

        for (var i = 0; i < maxValue; i++)
        {
            var hasName = layerData.TryGetValue($"{godotName}/layer_{i+1}", out var layerName);

            if (hasName || GenerateUnnamedValues)
            {
                layerName ??= $"Layer{i+1}";
                enumMembers.Add((layerName, i));
            }
        }
        
        var layers = string.Join("\n\t", enumMembers.Select(FormatEnumEntry));
        var maxValueBits = maxValue == 32 ? uint.MaxValue : (1u << maxValue) - 1;
        
        var text =
            $$"""
              using System;
              namespace GodotLib.ProjectConstants;

              [Flags]
              public enum {{cSharpName}} : uint
              {
                  {{layers}}
                  
                  ///Max value
                  All = {{maxValueBits}}
              }
              """;

        context.AddSource($"{cSharpName}.generated.cs", text);
    }

    private static string FormatEnumEntry((string name, int value) layer)
    {
        var text =  
            $"""
             ///Layer {layer.value + 1} (bit {layer.value})
                 {layer.name} = {1 << layer.value},
             """;

        return text;
    }

    private Dictionary<string, string> ExtractLayerNames(string content)
    {
        var layerNames = new Dictionary<string, string>();
        
        if (string.IsNullOrWhiteSpace(content))
        {
            return layerNames;
        }

        var pattern = @"\[layer_names\]\s*(?<content>[\s\S]*?)(?:\[(?!layer_names\]))|$";
        var match = Regex.Match(content, pattern, RegexOptions.IgnoreCase);

        if (match.Success)
        {
            var sectionContent = match.Groups["content"].Value;
            var keyValuePattern = @"^(?<key>[^\r\n=]+)\s*=\s*""?(?<value>[^""]*)""?$";
            var keyValueMatches = Regex.Matches(sectionContent, keyValuePattern, RegexOptions.Multiline);

            foreach (Match keyValueMatch in keyValueMatches)
            {
                var key = keyValueMatch.Groups["key"].Value;
                var value = keyValueMatch.Groups["value"].Value;
                layerNames[key] = value;
            }
        }

        return layerNames;
    }
}