using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GodotLib.Analyzers.SourceGenerators;

[Generator]
public class FlagsEnumExtensionsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Filter enums that have the [Flags] attribute
        var flagsEnums = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is EnumDeclarationSyntax,
                transform: static (ctx, _) => GetFlagsEnumOrNull(ctx))
            .Where(static enumInfo => enumInfo is not null)
            .Select(static (enumInfo, _) => enumInfo!);

        // Generate extension classes for each flags enum
        context.RegisterSourceOutput(flagsEnums, static (spc, enumInfo) =>
        {
            var source = GenerateExtensionClass(enumInfo.Value.Symbol, enumInfo.Value.Namespace);
            spc.AddSource($"{enumInfo.Value.Symbol.Name}Ext.generated.cs", source);
        });
    }

    private static (INamedTypeSymbol Symbol, string? Namespace)? GetFlagsEnumOrNull(GeneratorSyntaxContext context)
    {
        var enumDeclaration = (EnumDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(enumDeclaration) is not INamedTypeSymbol enumSymbol)
            return null;

        // Ignore private enums
        if (enumSymbol.DeclaredAccessibility == Accessibility.Private)
            return null;

        // Check if enum has [Flags] attribute
        var hasFlagsAttribute = enumSymbol.GetAttributes().Any(attr =>
            attr.AttributeClass?.Name is "FlagsAttribute" or "Flags");

        if (!hasFlagsAttribute)
            return null;

        var namespaceName = enumSymbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : enumSymbol.ContainingNamespace.ToDisplayString();

        return (enumSymbol, namespaceName);
    }

    private static string GenerateExtensionClass(INamedTypeSymbol enumSymbol, string? namespaceName)
    {
        var enumName = enumSymbol.Name;

        var sb = new StringBuilder();
        sb.AppendLine("using System.Runtime.CompilerServices;");
        sb.AppendLine();

        if (!string.IsNullOrEmpty(namespaceName))
        {
            sb.AppendLine($"namespace {namespaceName};");
            sb.AppendLine();
        }

        sb.AppendLine($"public static class {enumName}Ext");
        sb.AppendLine("{");

        // Generate Get method
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"    public static bool Get(this {enumName} mask, {enumName} flag)");
        sb.AppendLine("    {");
        sb.AppendLine("        return (mask & flag) != 0;");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Generate Set method
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"    public static void Set(this ref {enumName} mask, {enumName} flag, bool value)");
        sb.AppendLine("    {");
        sb.AppendLine("        mask = (value ? mask | flag : mask & ~flag);");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
