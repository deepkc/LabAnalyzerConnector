namespace LabAnalyzerConnector.Application.Events;

public sealed class DashboardEvent
{
    public DashboardEventType Type { get; init; }

    public Guid AnalyzerId { get; init; }

    public string AnalyzerName { get; init; } = "";

    public string Message { get; init; } = "";

    public DateTime Time { get; init; }
        = DateTime.UtcNow;
}