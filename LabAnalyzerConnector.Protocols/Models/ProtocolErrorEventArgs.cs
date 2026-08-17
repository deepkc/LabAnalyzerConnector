namespace LabAnalyzerConnector.Protocols.Models;

public sealed class ProtocolErrorEventArgs
    : EventArgs
{
    public Guid AnalyzerId { get; }

    public Exception Exception { get; }

    public DateTime OccurredAtUtc { get; }

    public ProtocolErrorEventArgs(
        Guid analyzerId,
        Exception exception)
    {
        AnalyzerId = analyzerId;

        Exception = exception;

        OccurredAtUtc =
            DateTime.UtcNow;
    }
}