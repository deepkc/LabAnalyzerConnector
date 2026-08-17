namespace LabAnalyzerConnector.Communication.Managers.Events;

public sealed class AnalyzerDataReceivedEventArgs
    : EventArgs
{
    public Guid AnalyzerId { get; }

    public string Data { get; }

    public DateTime ReceivedAtUtc { get; }

    public AnalyzerDataReceivedEventArgs(
        Guid analyzerId,
        string data)
    {
        AnalyzerId = analyzerId;

        Data = data;

        ReceivedAtUtc =
            DateTime.UtcNow;
    }
}