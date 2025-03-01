using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace GodotLib.Analyzers.SourceGenerators;

#pragma warning disable RS2000

public static class GeneratorLogger
{
    private static List<string> localLog = new();
    
    private static readonly DiagnosticDescriptor logDescriptor = new(

        id: "GENLOG",
        title: "Source Generator Log",
        messageFormat: "{0}",
        category: "SourceGenerator",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true
    );
    
    private static readonly DiagnosticDescriptor warnDescriptor = new(
        id: "GENWARN",
        title: "Source Generator Warning",
        messageFormat: "{0}",
        category: "SourceGenerator",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );
    
    private static readonly DiagnosticDescriptor errorDescriptor = new(
        id: "GENERR",
        title: "Source Generator Error",
        messageFormat: "{0}",
        category: "SourceGenerator",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static void Log(this GeneratorExecutionContext context, string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        Log(context, message, filePath, lineNumber, logDescriptor);
    }
    
    public static void LogWarning(this GeneratorExecutionContext context, string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        Log(context, message, filePath, lineNumber, warnDescriptor);
    }
    
    public static void LogError(this GeneratorExecutionContext context, string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        Log(context, message, filePath, lineNumber, errorDescriptor);
    }

    public static void DumpLogsToFile(this GeneratorExecutionContext context, [CallerFilePath] string filePath = "")
    {
        var sb = new StringBuilder();
        sb.AppendLine("/*");
        foreach (var log in localLog)
        {
            sb.AppendLine(log);
            sb.AppendLine();
        }
        sb.AppendLine("*/");
        context.AddSource($"Log.log", sb.ToString());
        localLog.Clear();
    }

    private static void Log(GeneratorExecutionContext context, string message, string filePath, int lineNumber, DiagnosticDescriptor descriptor)
    {
        var linePosition = new LinePositionSpan(new LinePosition(lineNumber-1, 0), new LinePosition(lineNumber-1, 0) );
        context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.Create(filePath, new TextSpan(), linePosition), message));

        localLog.Add($"[{filePath}:{lineNumber}] [{descriptor.DefaultSeverity}]: {message}");
    }


}