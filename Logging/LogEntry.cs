namespace GodotLib.Logging;

public readonly struct LogEntry(int index, TimeSpan timestamp, string loggerName, SeverityLevel severity, string message)
{
    public readonly int Index = index;
    public readonly TimeSpan Timestamp = timestamp;
    public readonly string LoggerName = loggerName;
    public readonly SeverityLevel Severity = severity;
    public readonly string Message = message;

    public string GetFormattedText(bool withSeverity = true)
    {
        if (withSeverity) return $@"[{timestamp:h\:mm\:ss\.fff}] [{LoggerName}] [{severity}] {message}";
        else return $@"[{timestamp:h\:mm\:ss\.fff}] [{LoggerName}] {Message}";
    }
}