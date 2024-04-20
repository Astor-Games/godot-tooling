using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DotNetTooling.SourceGenerators;

[Generator]
public class GodotLayerEnumGenerator : IIncrementalGenerator
{
    private static bool generateUnnamedValues = false;
    
    private readonly Dictionary<string, string> patterns = new()
    {
        {"2d_physics", "PhysicsLayers2D"},
        {"2d_navigation", "NavigationLayers2D"},
        {"2d_render", "RenderLayers2D"},
        {"3d_physics", "PhysicsLayers3D"},
        {"3d_navigation", "NavigationLayers3D"},
        {"3d_render", "RenderLayers3D"}
    };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var projectFileProvider = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith("project.godot"))
            .Select((file, cancellationToken) => file.GetText(cancellationToken)!.ToString())
            .WithTrackingName("GodotProjectFile");

        var namespaceProvider = context.AnalyzerConfigOptionsProvider.Select((provider, _) => 
            provider.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace) ? rootNamespace : "DefaultProject");
        
        var valueProvider = projectFileProvider.Combine(namespaceProvider);
        
        context.RegisterSourceOutput(valueProvider, Generate);

    }

    private void Generate(SourceProductionContext context, (string text, string namespaceName) data)
    {
        var layersData = ParseProjectFile(data.text);
        var namespaceName = $"{data.namespaceName}.ProjectConstants";
        
        foreach (var pattern in patterns)
        {
            GenerateLayersFile(context, pattern.Value, pattern.Key, namespaceName, layersData);
        }
    }

    private void GenerateLayersFile(SourceProductionContext context, string groupName, string groupKey,
        string namespaceName, Dictionary<string, string> layerData)
    {
        // Generate the C# enum based on the layer data
        // Create an enum declaration with the [Flags] attribute
        var enumSyntax = EnumDeclaration(groupName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddBaseListTypes(SimpleBaseType(ParseTypeName("uint")))
            .AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(ParseName("Flags")))));

        EnumMemberDeclarationSyntax enumMemberSyntax;
        
        for (var i = 1; i <= 32; i++)
        {
            var hasName = layerData.TryGetValue($"{groupKey}/layer_{i}", out var layerName);

            if (hasName || generateUnnamedValues)
            {
                layerName ??= $"Layer{i}";
                var layerValue = 1u << i - 1;

                enumMemberSyntax = EnumMemberDeclaration(layerName)
                    .WithEqualsValue(EqualsValueClause(ParseExpression(layerValue.ToString())));

                enumSyntax = enumSyntax.AddMembers(enumMemberSyntax);
            }
        }
        
        enumMemberSyntax = EnumMemberDeclaration("All")
            .WithEqualsValue(EqualsValueClause(ParseExpression(uint.MaxValue.ToString())));
        enumSyntax = enumSyntax.AddMembers(enumMemberSyntax);
        
        // Create a compilation unit and namespace
        var namespaceSyntax = NamespaceDeclaration(ParseName(namespaceName))
            .AddMembers(enumSyntax);

        var compilationUnitSyntax = CompilationUnit()
            .AddUsings(UsingDirective(ParseName("System")), UsingDirective(ParseName("System.ComponentModel")))
            .AddMembers(namespaceSyntax);

        // Create a SyntaxTree with the generated code
        var syntaxTree = SyntaxTree(compilationUnitSyntax);

        // Add the generated code to the compilation
        context.AddSource($"{groupName}.generated.cs", syntaxTree.GetRoot().NormalizeWhitespace().ToFullString());
    }

    private Dictionary<string, string> ParseProjectFile(string content)
    {
        var layerNames = new Dictionary<string, string>();

        // Define the regular expression pattern for the [layer_names] section
        var pattern = @"\[layer_names\]\s*(?<content>[\s\S]*?)(?:\[(?!layer_names\]))|$";

        var match = Regex.Match(content, pattern, RegexOptions.IgnoreCase);

        if (match.Success)
        {
            // Extract the content of the [layer_names] section
            var sectionContent = match.Groups["content"].Value;

            // Define the pattern for key-value pairs
            var keyValuePattern = @"^(?<key>[^\r\n=]+)\s*=\s*""?(?<value>[^""]*)""?$";

            // Match key-value pairs within the [layer_names] section
            var keyValueMatches = Regex.Matches(sectionContent, keyValuePattern, RegexOptions.Multiline);

            // Populate the layerNames dictionary with matched values
            foreach (Match keyValueMatch in keyValueMatches)
            {
                var key = keyValueMatch.Groups["key"].Value;
                var value = keyValueMatch.Groups["value"].Value;

                // Add key-value pair to the dictionary
                layerNames[key] = value;
            }
        }

        return layerNames;
    }
}