using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;

namespace GodotLib.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BaseMethodCallCodeFixProvider)), Shared]
public class BaseMethodCallCodeFixProvider : CodeFixProvider
{
    private const string title = "Add base method invocation";

    public sealed override ImmutableArray<string> FixableDiagnosticIds => 
        ImmutableArray.Create(RequiredBaseMethodCallAnalyzer.DiagnosticId);

    public sealed override FixAllProvider GetFixAllProvider()
    {
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        return WellKnownFixAllProviders.BatchFixer;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Register a code action that will invoke the fix.
        context.RegisterCodeFix(
            CodeAction.Create(
                title: title,
                createChangedDocument: c => AddBaseMethodCallAsync(context.Document, diagnosticSpan, c),
                equivalenceKey: title),
            diagnostic);
    }

    private async Task<Document> AddBaseMethodCallAsync(Document document, TextSpan diagnosticSpan, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        var node = root?.FindNode(diagnosticSpan) as MethodDeclarationSyntax;

        var args = new List<ArgumentSyntax>();
        foreach (var param in node.ParameterList.Parameters)
        {
            args.Add(SyntaxFactory.Argument(SyntaxFactory.ParseExpression(param.Identifier.ValueText)));
        }

        var argsList = SyntaxFactory.SeparatedList(args);

        var exprStatement = SyntaxFactory.ExpressionStatement(
            SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.BaseExpression(),
                    SyntaxFactory.Token(SyntaxKind.DotToken),
                    SyntaxFactory.IdentifierName(node.Identifier.ToString())
                ),
                SyntaxFactory.ArgumentList(argsList)
            ),
            SyntaxFactory.Token(SyntaxKind.SemicolonToken)
        );

        var newBodyStatements = SyntaxFactory.Block(node.Body.Statements.Insert(0, exprStatement));
        var newRoot = root.ReplaceNode(node.Body, newBodyStatements).WithAdditionalAnnotations(Simplifier.Annotation);

        return document.WithSyntaxRoot(newRoot);
    }
}