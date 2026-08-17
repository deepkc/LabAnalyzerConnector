using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Logging.Models;

public class LogEntry
{
    public DateTime Timestamp { get; set; } =
        DateTime.UtcNow;

    public LogLevel Level { get; set; }

    public string Source { get; set; } =
        string.Empty;

    public Guid? AnalyzerId { get; set; }

    public string Message { get; set; } =
        string.Empty;

    public Exception? Exception { get; set; }
}