using LabAnalyzerConnector.Core.Enums;
using LabAnalyzerConnector.Logging.Models;

namespace LabAnalyzerConnector.Logging.Abstractions;

public interface ILoggerService
{
    event EventHandler<LogEntry>? LogWritten;

    void Log(
        LogLevel level,
        string source,
        string message,
        Guid? analyzerId = null,
        Exception? exception = null);

    void Trace(
        string source,
        string message,
        Guid? analyzerId = null);

    void Debug(
        string source,
        string message,
        Guid? analyzerId = null);

    void Information(
        string source,
        string message,
        Guid? analyzerId = null);

    void Warning(
        string source,
        string message,
        Guid? analyzerId = null);

    void Error(
        string source,
        string message,
        Exception? exception = null,
        Guid? analyzerId = null);

    void Critical(
        string source,
        string message,
        Exception? exception = null,
        Guid? analyzerId = null);

    IReadOnlyCollection<LogEntry> GetLogs();

    void Clear();
}