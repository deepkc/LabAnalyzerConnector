using LabAnalyzerConnector.Domain.Entities;

namespace LabAnalyzerConnector.Application.Processing.Events;

public sealed class NormalizedMessageProcessedEventArgs
    : EventArgs
{
    public NormalizedLabMessage Message { get; }

    public NormalizedMessageProcessedEventArgs(
        NormalizedLabMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        Message = message;
    }
}