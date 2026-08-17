using LabAnalyzerConnector.Core.Models;

namespace LabAnalyzerConnector.Application.Interfaces;

public interface ILisAdapter
{
    string Name { get; }

    bool IsConnected { get; }

    Task ConnectAsync(
        CancellationToken cancellationToken = default);

    Task DisconnectAsync();

    Task SendOrderAsync(
        LabOrder order,
        CancellationToken cancellationToken = default);

    event EventHandler<LabOrder>? OrderReceived;

    event EventHandler<string>? ResultReceived;

    event EventHandler<Exception>? ErrorOccurred;
}