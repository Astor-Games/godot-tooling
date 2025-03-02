using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GodotLib.Analyzers.SourceGenerators.Utils;

public static class GodotProjectHelper
{
    public static string ReadGodotProject(GeneratorExecutionContext context)
    {
        var projectText = string.Empty;
        try
        {
            var projectFile = context.AdditionalFiles
                .First(file => file.Path.EndsWith("project.godot"));
            
            context.Log($"project file path: {projectFile.Path}");

            projectText = projectFile.GetText()?.ToString();
            context.Log($"project file content: \n\n{projectText} \n\n");
        }
        catch (Exception e)
        {
            context.Log($"Failed to read project.godot. Error: {e.Message}");
        }

        return projectText;
    }
}