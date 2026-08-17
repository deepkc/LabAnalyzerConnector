using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Communication.Managers.Events;

public sealed class ConnectionStatusChangedEventArgs
    : EventArgs
{
    public Guid AnalyzerId { get; }

    public ConnectionStatus Status { get; }

    public ConnectionStatusChangedEventArgs(
        Guid analyzerId,
        ConnectionStatus status)
    {
        AnalyzerId = analyzerId;
        Status = status;
    }
}