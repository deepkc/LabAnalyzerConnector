namespace LabAnalyzerConnector.Application.Events;

public sealed class DashboardEventArgs
    : EventArgs
{
    public DashboardEventArgs(
        DashboardEvent dashboardEvent)
    {
        Event = dashboardEvent;
    }

    public DashboardEvent Event { get; }
}