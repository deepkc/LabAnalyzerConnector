namespace LabAnalyzerConnector.Communication.Managers.Events;

public sealed class AnalyzerErrorEventArgs
    : EventArgs
{
    public Guid AnalyzerId { get; }

    public Exception Exception { get; }

    public DateTime OccurredAtUtc { get; }

    public AnalyzerErrorEventArgs(
        Guid analyzerId,
        Exception exception)
    {
        AnalyzerId = analyzerId;

        Exception = exception;

        OccurredAtUtc =
            DateTime.UtcNow;
    }
}