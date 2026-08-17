using System.Net.Sockets;
using System.Text;
using LabAnalyzerConnector.Communication.Abstractions;
using LabAnalyzerConnector.Communication.Exceptions;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Communication.Tcp;

public sealed class TcpClientConnection : IAnalyzerConnection
{
    private readonly Guid _analyzerId;
    private readonly TcpConfiguration _configuration;
    private readonly CommunicationDirection _direction;

    private TcpClient? _tcpClient;
    private NetworkStream? _networkStream;
    private CancellationTokenSource? _receiveCancellation;

    public Guid AnalyzerId => _analyzerId;
    public CommunicationDirection Direction =>
    _direction;

    public ConnectionStatus Status { get; private set; } =
        ConnectionStatus.Disconnected;

    public event EventHandler<ConnectionStatus>? StatusChanged;

    public event EventHandler<string>? DataReceived;

    public event EventHandler<Exception>? ErrorOccurred;

    public TcpClientConnection(
    Guid analyzerId,
    TcpConfiguration configuration,
    CommunicationDirection direction)
    {
        _analyzerId = analyzerId;
        _configuration = configuration;
        _direction = direction;
    }

    public async Task ConnectAsync(
        CancellationToken cancellationToken = default)
    {
        if (Status == ConnectionStatus.Connected)
        {
            return;
        }

        try
        {
            SetStatus(ConnectionStatus.Connecting);

            _tcpClient = new TcpClient();

            using var timeoutCts =
                new CancellationTokenSource(
                    _configuration.ConnectionTimeoutMilliseconds);

            using var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    timeoutCts.Token);

            await _tcpClient.ConnectAsync(
                _configuration.RemoteIpAddress,
             _configuration.RemotePort,
                linkedCts.Token);

            _networkStream = _tcpClient.GetStream();

            SetStatus(ConnectionStatus.Connected);

            _receiveCancellation =
                new CancellationTokenSource();

            _ = ReceiveLoopAsync(
                _receiveCancellation.Token);
        }
        catch (OperationCanceledException)
        {
            SetStatus(ConnectionStatus.Disconnected);

            throw;
        }
        catch (Exception ex)
        {
            SetStatus(ConnectionStatus.Error);

            var connectionException =
                new ConnectionException(
                    $"Failed to connect to {_configuration.RemoteIpAddress}:{_configuration.RemotePort}.",
                    ex);

            ErrorOccurred?.Invoke(
                this,
                connectionException);

            throw connectionException;
        }
    }

    public async Task DisconnectAsync()
    {
        try
        {
            _receiveCancellation?.Cancel();

            if (_networkStream is not null)
            {
                await _networkStream.DisposeAsync();
                _networkStream = null;
            }

            _tcpClient?.Close();
            _tcpClient?.Dispose();

            _tcpClient = null;

            SetStatus(ConnectionStatus.Disconnected);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(
                this,
                ex);

            SetStatus(ConnectionStatus.Error);
        }
    }

    public async Task SendAsync(
        string data,
        CancellationToken cancellationToken = default)
    {
        if (_networkStream is null ||
     _tcpClient is null ||
     !_tcpClient.Connected)
        {
            throw new ConnectionException(
                "TCP connection is not active.");
        }

        try
        {
            byte[] bytes =
     Encoding.ASCII.GetBytes(data);

            System.Diagnostics.Debug.WriteLine("========== SENDING ==========");
            System.Diagnostics.Debug.WriteLine(data);
            System.Diagnostics.Debug.WriteLine("=============================");

            await _networkStream.WriteAsync(
                bytes,
                cancellationToken);

            await _networkStream.FlushAsync(
                cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(
                this,
                ex);

            SetStatus(ConnectionStatus.Error);

            throw new ConnectionException(
                "Failed to send data over TCP.",
                ex);
        }
    }

    private async Task ReceiveLoopAsync(
        CancellationToken cancellationToken)
    {
        if (_networkStream is null)
        {
            return;
        }

        byte[] buffer = new byte[4096];

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int bytesRead =
                    await _networkStream.ReadAsync(
                        buffer,
                        cancellationToken);

                if (bytesRead == 0)
                {
                    SetStatus(
                        ConnectionStatus.Disconnected);

                    break;
                }

                string data =
                    Encoding.ASCII.GetString(
                        buffer,
                        0,
                        bytesRead);

                DataReceived?.Invoke(
                    this,
                    data);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when the connection is stopped.
        }
        catch (Exception ex)
        {
            SetStatus(ConnectionStatus.Error);

            ErrorOccurred?.Invoke(
                this,
                ex);
        }
    }

    private void SetStatus(
        ConnectionStatus status)
    {
        if (Status == status)
        {
            return;
        }

        Status = status;

        StatusChanged?.Invoke(
            this,
            status);
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();

        _receiveCancellation?.Dispose();
    }
}