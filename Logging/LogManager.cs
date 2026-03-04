using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using GodotLib.Debug;
using GodotLib.Util;
using Console = System.Console;

namespace GodotLib.Logging;

public partial class LogManager : Node
{
    public static readonly Color TraceColor = Color.Color8(129, 129, 152);
    public static readonly Color InfoColor = Color.Color8(218, 229, 241);
    public static readonly Color WarningColor = Color.Color8(255, 222, 51);
    public static readonly Color ErrorColor = Color.Color8(230, 28, 0);

    public static readonly Texture2D TraceIcon = Load<Texture2D>("uid://cjcalpfywbq6e");
    public static readonly Texture2D InfoIcon = Load<Texture2D>("uid://va31x4jxjab0");
    public static readonly Texture2D WarningIcon = Load<Texture2D>("uid://b1v7xi4in8d8k");
    public static readonly Texture2D ErrorIcon = Load<Texture2D>("uid://dxqoboahig0j5");
    
    public static LogManager Instance {get; private set; }
    public event Action<string> LoggerRegistered;
    
   // [Flags]
    public enum LogOutputs
    {
        GodotConsole = 1 << 0,
        DotNetConsole = 1 << 1,
        File = 1 << 2
    }
    public LogOutputs Outputs;
    
    private readonly Lock printLock = new();
    private readonly Stopwatch stopwatch;
    private int logIdx = 0;
    private readonly Dictionary<string, Logger> loggers = new();
    
    // Make it lazy to avoid an initialization interlock
    private Logger log => field ??= Logger.For<LogManager>();

    // File output support
    private StreamWriter fileWriter;
    private Channel<string> fileChannel;
    private CancellationTokenSource fileCts;
    private readonly ManualResetEventSlim fileWriterCompleted = new(true);

    // In-memory log storage
    private const int MaxLogEntries = 10000; // Circular buffer size
    private readonly CircularBuffer<LogEntry> logBuffer = new(MaxLogEntries);
    private readonly Lock logBufferLock = new();

    public LogManager()
    {
        Assertions.AssertNull(Instance);
        
        Outputs = LogOutputs.DotNetConsole; // Add file if appropriate
        stopwatch = Stopwatch.StartNew();

        if (OS.HasCmdlineArg("logfile"))
        {
            EnableFileOutput();
        }
        Instance = this;
    }

    public void RegisterLogger(Logger logger)
    {
        if (!loggers.TryAdd(logger.Name, logger))
        {
            throw new Exception($"Logger '{logger.Name}' was already registered");
        }
        
        LoggerRegistered?.Invoke(logger.Name);
    }

    public IEnumerable<string> GetLoggers()
    {
        return loggers.Keys;
    }

    public bool TryGetLogger(string name, out Logger logger)
    {
        lock (this)
        {
            return loggers.TryGetValue(name, out logger);
        }
    }
    
    public void AddLog(Logger logger, SeverityLevel severity, string message)
    {
        var index = logIdx++;
        var timestamp = stopwatch.Elapsed;
        var entry = new LogEntry(index, timestamp, logger.Name, severity, message);
        var formattedMessage = entry.GetFormattedText();

        // Store in circular buffer
        lock (logBufferLock)
        {
            logBuffer.PushBack(entry);
        }

        if (Outputs.HasFlag(LogOutputs.File))
        {
            fileChannel?.Writer.TryWrite(formattedMessage);
        }

        if (Outputs.HasFlag(LogOutputs.GodotConsole))
        {
            Print(formattedMessage);
        }

        if (Outputs.HasFlag(LogOutputs.DotNetConsole))
        {
            var consoleColor = severity switch
            {
                SeverityLevel.Trace => ConsoleColor.Gray,
                SeverityLevel.Info => ConsoleColor.Black,
                SeverityLevel.Warning => ConsoleColor.Yellow,
                SeverityLevel.Error => ConsoleColor.Red,
                _ => ConsoleColor.White
            };

            lock (printLock)
            {
                Console.ForegroundColor = consoleColor;
                Console.WriteLine(formattedMessage);
                Console.ResetColor();
            }
        }
    }
    public LogEntry GetLog(int idx)
    {
        lock (logBufferLock)
        {
            // Calculate the actual buffer index from the logical log index
            var oldestLogIdx = logIdx - logBuffer.Size;
            if (idx < oldestLogIdx)
            {
                log.Warning($"Log entry {idx} no longer available");
                return default;
            }
            
            var bufferOffset = idx - oldestLogIdx;
            return logBuffer[bufferOffset];
        }
    }
    
    public CircularBuffer<LogEntry>.RangeView GetLogsFrom(int startIdx)
    {
        lock (logBufferLock)
        {
            var oldestLogIdx = logIdx - logBuffer.Size;
            
            // If requesting logs older than what we have, start from oldest
            if (startIdx < oldestLogIdx)
            {
                startIdx = oldestLogIdx;
            }
            
            // If requesting logs beyond current, return empty
            if (startIdx >= logIdx)
            {
                return new CircularBuffer<LogEntry>.RangeView(logBuffer, 0, 0);
            }
            
            // Calculate buffer offsets
            var startOffset = startIdx - oldestLogIdx;
            var endOffset = logBuffer.Size;
            
            return logBuffer[startOffset..endOffset];
        }
    }

    public CircularBuffer<LogEntry>.RangeView GetAllLogs()
    {
        lock (logBufferLock)
        {
            return logBuffer[..];
        }
    }
    
    public static Color GetColor(SeverityLevel severity)
    {
        return severity switch
        {
            SeverityLevel.Trace => TraceColor,
            SeverityLevel.Info => InfoColor,
            SeverityLevel.Warning => WarningColor,
            SeverityLevel.Error => ErrorColor,
        };
    }
    
    public static Texture2D GetIcon(SeverityLevel severity)
    {
        return severity switch
        {
            SeverityLevel.Trace => TraceIcon,
            SeverityLevel.Info => InfoIcon,
            SeverityLevel.Warning => WarningIcon,
            SeverityLevel.Error => ErrorIcon,
        };
    }
    
    public  void EnableFileOutput()
    {
        if (fileWriter != null) return; // Already enabled

        try
        {
            var filepath = ProjectSettings.GlobalizePath($"user://logs/turtles_{DateTime.Now:yyyy.MM.dd_HH.mm.ss}.log");
            fileWriter = new StreamWriter(filepath, append: false) { AutoFlush = false };
            fileChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
            fileCts = new CancellationTokenSource();

            // Start background task to write to file
            fileWriterCompleted.Reset();
            Task.Run(async () =>
            {
                try
                {
                    await FileWriterLoop(fileCts.Token);
                }
                finally
                {
                    fileWriterCompleted.Set();
                }
            }).Forget();

            Outputs |= LogOutputs.File;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to enable file logging: {ex.Message}");
        }
    }

    public void DisableFileOutput()
    {
        Outputs &= ~LogOutputs.File;
        FlushAndCloseFile();
    }

    /// <summary>
    /// Flushes all pending log messages and closes the file writer.
    /// Called automatically by LogManager on application exit.
    /// </summary>
    public void FlushAndCloseFile()
    {
        if (fileWriter == null) return;

        try
        {
            // Complete the channel to stop accepting new messages
            fileChannel?.Writer.Complete();

            // Wait for background task to finish writing remaining messages
            fileWriterCompleted.Wait(TimeSpan.FromSeconds(5));

            // Final flush and cleanup
            fileWriter?.Flush();
            fileWriter?.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during log file cleanup: {ex.Message}");
        }
        finally
        {
            fileWriter = null;
            fileCts?.Dispose();
            fileCts = null;
        }
    }

    private async Task FileWriterLoop(CancellationToken ct)
    {
        try
        {
            await foreach (var message in fileChannel.Reader.ReadAllAsync(ct))
            {
                await fileWriter.WriteLineAsync(message);
                await fileWriter.FlushAsync(ct);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown, flush any remaining buffered data
            await fileWriter.FlushAsync(ct);
        }
    }
    
    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest || what == NotificationExitTree || what == NotificationPredelete)
        {
            FlushAndCloseFile();
        }
    }
}