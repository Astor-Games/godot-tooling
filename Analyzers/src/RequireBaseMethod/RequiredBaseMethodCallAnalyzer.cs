using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace GodotLib.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2000:Add analyzer diagnostic IDs to analyzer release")]
public class RequiredBaseMethodCallAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "RequireBaseMethodCall";

    // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
    // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
    private static readonly string title = "Base method call required";
    private static readonly string messageFormat = "Overriden method '{0}' must call it's base class method";
    private static readonly string description = "Overriden method must call it's base class method.";
    private const string Category = "Usage";

    private static DiagnosticDescriptor rule = new(DiagnosticId, title, messageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterCompilationStartAction(AnalyzeMethodForBaseCall);
    }

    private static void AnalyzeMethodForBaseCall(CompilationStartAnalysisContext compilationStartContext)
    {
        compilationStartContext.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax mds)
        {
            return;
        }

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, mds) is not IMethodSymbol symbol)
        {
            return;
        }

        if (!symbol.IsOverride)
        {
            return;
        }

        if (symbol.OverriddenMethod == null)
        {
            return;
        }

        var overridenMethod = symbol.OverriddenMethod;
        var attrs = overridenMethod.GetAttributes();
        if (!attrs.Any(ad => string.Equals(ad.AttributeClass?.MetadataName, nameof(RequireBaseMethodCallAttribute), StringComparison.InvariantCultureIgnoreCase)))
        {
            return;
        }

        var overridenMethodName = overridenMethod.Name;
        var methodName = overridenMethodName;

        var invocations = mds.DescendantNodes().OfType<MemberAccessExpressionSyntax>().ToList();
        foreach (var inv in invocations)
        {
            var expr = inv.Expression;
            if ((SyntaxKind)expr.RawKind == SyntaxKind.BaseExpression)
            {
                if (expr.Parent is not MemberAccessExpressionSyntax memberAccessExpr)
                {
                    continue;
                }

                // compare exprSymbol and overridenMethod
                var exprMethodName = memberAccessExpr.Name.ToString();

                if (exprMethodName != overridenMethodName)
                {
                    continue;
                }

                var invokationExpr = memberAccessExpr.Parent as InvocationExpressionSyntax;
                if (invokationExpr == null)
                {
                    continue;
                }
                var exprMethodArgs = invokationExpr.ArgumentList.Arguments.ToList();
                var ovrMethodParams = overridenMethod.Parameters.ToList();

                if (exprMethodArgs.Count != ovrMethodParams.Count)
                {
                    continue;
                }

                var paramMismatch = false;
                for (var i = 0; i < exprMethodArgs.Count; i++)
                {
                    var arg = exprMethodArgs[i];
                    var argType = ModelExtensions.GetTypeInfo(context.SemanticModel, arg.Expression);

                    var param = arg.NameColon != null ? 
                                ovrMethodParams.FirstOrDefault(p => p.Name.ToString() == arg.NameColon.Name.ToString()) : 
                                ovrMethodParams[i];

                    if (param == null || argType.Type != param.Type)
                    {
                        paramMismatch = true;
                        break;
                    }

                    exprMethodArgs.Remove(arg);
                    ovrMethodParams.Remove(param);
                    i--;
                }

                // If there are any parameters left without default value
                // then it is not the base method overload we are looking for
                if (ovrMethodParams.Any(p => p.HasExplicitDefaultValue))
                {
                    continue;
                }

                if (!paramMismatch)
                {
                    // If the actual arguments match with the method params
                    // then the base method invokation was found
                    // and there is no need to continue the search
                    return;
                }
            }
        }

        var diag = Diagnostic.Create(rule, mds.GetLocation(), methodName);
        context.ReportDiagnostic(diag);
    }
}