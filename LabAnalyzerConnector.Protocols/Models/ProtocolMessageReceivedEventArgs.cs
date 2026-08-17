namespace LabAnalyzerConnector.Protocols.Models;

public sealed class ProtocolMessageReceivedEventArgs
    : EventArgs
{
    public Guid AnalyzerId { get; }

    public string RawMessage { get; }

    public object ParsedMessage { get; }

    public DateTime ReceivedAtUtc { get; }

    public ProtocolMessageReceivedEventArgs(
        Guid analyzerId,
        string rawMessage,
        object parsedMessage)
    {
        AnalyzerId =
            analyzerId;

        RawMessage =
            rawMessage;

        ParsedMessage =
            parsedMessage;

        ReceivedAtUtc =
            DateTime.UtcNow;
    }
}