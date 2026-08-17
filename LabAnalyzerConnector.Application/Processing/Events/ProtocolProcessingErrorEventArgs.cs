namespace LabAnalyzerConnector.Application.Processing.Events;

public sealed class ProtocolProcessingErrorEventArgs
    : EventArgs
{
    public Guid AnalyzerId { get; }

    public Exception Exception { get; }

    public ProtocolProcessingErrorEventArgs(
        Guid analyzerId,
        Exception exception)
    {
        AnalyzerId = analyzerId;

        Exception =
            exception ??
            throw new ArgumentNullException(
                nameof(exception));
    }
}