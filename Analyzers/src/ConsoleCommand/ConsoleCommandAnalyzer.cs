using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace GodotLib.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2000:Add analyzer diagnostic IDs to analyzer release")]
public class ConsoleCommandAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "ConsoleCommandMustBeStatic";

    private static readonly string title = "ConsoleCommand must be applied to static methods";
    private static readonly string messageFormat = "Method '{0}' has [ConsoleCommand] attribute but is not static. Console commands must be static methods.";
    private static readonly string description = "Methods decorated with [ConsoleCommand] attribute must be static.";
    private const string Category = "Usage";

    private static DiagnosticDescriptor rule = new(
        DiagnosticId,
        title,
        messageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax methodDeclaration)
        {
            return;
        }

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, methodDeclaration) is not IMethodSymbol methodSymbol)
        {
            return;
        }

        // Check if method has [ConsoleCommand] attribute
        var hasConsoleCommandAttribute = false;
        foreach (var attribute in methodSymbol.GetAttributes())
        {
            if (string.Equals(attribute.AttributeClass?.Name, "ConsoleCommandAttribute", StringComparison.Ordinal) ||
                string.Equals(attribute.AttributeClass?.Name, "ConsoleCommand", StringComparison.Ordinal))
            {
                hasConsoleCommandAttribute = true;
                break;
            }
        }

        if (!hasConsoleCommandAttribute)
        {
            return;
        }

        // If method has the attribute but is not static, report diagnostic
        if (!methodSymbol.IsStatic)
        {
            var diagnostic = Diagnostic.Create(rule, methodDeclaration.Identifier.GetLocation(), methodSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
