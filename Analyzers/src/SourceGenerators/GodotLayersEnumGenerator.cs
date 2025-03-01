#define DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace GodotLib.Analyzers.SourceGenerators;

[Generator]
public class GodotLayerEnumGenerator : ISourceGenerator
{
    private const bool GenerateUnnamedValues = false;

    private readonly Dictionary<string, string> patterns = new()
    {
        { "2d_physics", "PhysicsLayers2D" },
        { "3d_physics", "PhysicsLayers3D" },
        { "2d_navigation", "NavigationLayers2D" },
        { "3d_navigation", "NavigationLayers3D" },
        { "2d_render", "RenderLayers2D" },
        { "3d_render", "RenderLayers3D" }
    };

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization needed for ISourceGenerator
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var layersData = new Dictionary<string, string>();
        try
        {
            var additionalFiles = string.Join(", ", context.AdditionalFiles.Select(file => file.Path));
            context.Log($"Adding files: {additionalFiles}");
            
            var projectFile = context.AdditionalFiles
                .First(file => file.Path.EndsWith("project.godot"));
            
            context.Log($"project file path: {projectFile.Path}");

            var projectText = projectFile.GetText()?.ToString();
            context.Log($"project file content: \n\n{projectText} \n\n");
            layersData = ParseProjectFile(projectText);
        }
        catch (Exception e)
        {
            context.Log($"Failed to read the layers data from project.godot. Error: {e.Message}");
        }
        finally
        {
            foreach (var pattern in patterns)
            {
                GenerateLayersFile(context, pattern.Value, pattern.Key, layersData);
            }
            
            context.DumpLogsToFile();
        }
    }

    private void GenerateLayersFile(GeneratorExecutionContext context, string groupName, string groupKey, Dictionary<string, string> layerData)
    {
        var enumMembers = new List<(string name, int value)>();

        for (var i = 0; i < 32; i++)
        {
            var hasName = layerData.TryGetValue($"{groupKey}/layer_{i+1}", out var layerName);

            if (hasName || GenerateUnnamedValues)
            {
                layerName ??= $"Layer{i+1}";
                enumMembers.Add((layerName, i));
            }
        }
        
        var layers = string.Join("\n\t", enumMembers.Select(FormatEnumEntry));

        var text =
            $$"""
              using System;
              namespace GodotLib.ProjectConstants;

              [Flags]
              public enum {{groupName}} : uint
              {
                  {{layers}}
                  All = uint.MaxValue
              }
              """;

        context.AddSource($"{groupName}.generated.cs", text);
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

    private Dictionary<string, string> ParseProjectFile(string content)
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