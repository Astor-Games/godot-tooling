#define DEBUG

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GodotLib.Analyzers.SourceGenerators.Utils;
using Microsoft.CodeAnalysis;

namespace GodotLib.Analyzers.SourceGenerators;

[Generator]
public class GodotInputActionsGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization needed for ISourceGenerator
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var projectText = GodotProjectHelper.ReadGodotProject(context);
        var actionNames = ExtractInputActions(projectText);
       
        GenerateActionsFile(context, actionNames);
    }

    private void GenerateActionsFile(GeneratorExecutionContext context, List<string> names)
    {
        var members = string.Join("\n\t", names.Select(FormatAction));

        var text =
            $$"""
              using System;
              using Godot;
              namespace GodotLib.ProjectConstants;

              public class InputActions
              {
                  {{members}}
              }
              """;

        context.AddSource("InputActions.generated.cs", text);
    }

    private static string FormatAction(string name)
    {
        return $"""public static readonly StringName {name.SnakeToPascalCase()} = "{name}";""";
    }

    private List<string> ExtractInputActions(string content)
    {
        var inputActions = new List<string>();

        if (string.IsNullOrWhiteSpace(content))
        {
            return inputActions;
        }

        // Match the [input] section
        var inputSectionPattern = @"\[input\]\s*(?<content>[\s\S]*?)(?:\n\[|$)";
        var match = Regex.Match(content, inputSectionPattern, RegexOptions.IgnoreCase);

        if (match.Success)
        {
            var sectionContent = match.Groups["content"].Value;

            // Match each action name (everything before '=' followed by '{')
            var actionPattern = @"^(?<action>[^\s=]+)\s*=\s*\{";

            var actionMatches = Regex.Matches(sectionContent, actionPattern, RegexOptions.Multiline);

            foreach (Match actionMatch in actionMatches)
            {
                var actionName = actionMatch.Groups["action"].Value.Trim();
                inputActions.Add(actionName);
            }
        }

        return inputActions;
    }
}