using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Communication.Abstractions;

public interface IAnalyzerConnection : IAsyncDisposable
{
    Guid AnalyzerId { get; }

    ConnectionStatus Status { get; }

    CommunicationDirection Direction { get; }

    event EventHandler<ConnectionStatus>? StatusChanged;

    event EventHandler<string>? DataReceived;

    event EventHandler<Exception>? ErrorOccurred;

    Task ConnectAsync(
        CancellationToken cancellationToken = default);

    Task DisconnectAsync();

    Task SendAsync(
        string data,
        CancellationToken cancellationToken = default);
}