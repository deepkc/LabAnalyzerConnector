namespace LabAnalyzerConnector.Application.Events;

public sealed class DashboardEventBus
{
    public event EventHandler<DashboardEventArgs>?
        EventPublished;

    public void Publish(
        DashboardEvent dashboardEvent)
    {
        EventPublished?.Invoke(
            this,
            new DashboardEventArgs(dashboardEvent));


    }


}