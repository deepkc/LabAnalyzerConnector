using LabAnalyzerConnector.Core.Enums;
using LabAnalyzerConnector.Logging.Abstractions;
using LabAnalyzerConnector.Logging.Models;

namespace LabAnalyzerConnector.Logging.Services;

public sealed class LoggerService : ILoggerService
{
    private readonly List<LogEntry> _logs = new();
    private readonly object _lock = new();

    public event EventHandler<LogEntry>? LogWritten;

    public void Log(
        LogLevel level,
        string source,
        string message,
        Guid? analyzerId = null,
        Exception? exception = null)
    {
        var entry = new LogEntry
        {
            Timestamp = DateTime.UtcNow,
            Level = level,
            Source = source,
            AnalyzerId = analyzerId,
            Message = message,
            Exception = exception
        };

        lock (_lock)
        {
            _logs.Add(entry);
        }

        LogWritten?.Invoke(this, entry);
    }

    public void Trace(
        string source,
        string message,
        Guid? analyzerId = null)
    {
        Log(LogLevel.Trace, source, message, analyzerId);
    }

    public void Debug(
        string source,
        string message,
        Guid? analyzerId = null)
    {
        Log(LogLevel.Debug, source, message, analyzerId);
    }

    public void Information(
        string source,
        string message,
        Guid? analyzerId = null)
    {
        Log(LogLevel.Information, source, message, analyzerId);
    }

    public void Warning(
        string source,
        string message,
        Guid? analyzerId = null)
    {
        Log(LogLevel.Warning, source, message, analyzerId);
    }

    public void Error(
        string source,
        string message,
        Exception? exception = null,
        Guid? analyzerId = null)
    {
        Log(
            LogLevel.Error,
            source,
            message,
            analyzerId,
            exception);
    }

    public void Critical(
        string source,
        string message,
        Exception? exception = null,
        Guid? analyzerId = null)
    {
        Log(
            LogLevel.Critical,
            source,
            message,
            analyzerId,
            exception);
    }

    public IReadOnlyCollection<LogEntry> GetLogs()
    {
        lock (_lock)
        {
            return _logs.ToList().AsReadOnly();
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _logs.Clear();
        }
    }
}