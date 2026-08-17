using System.Net.Sockets;
using System.Text;

namespace LabAnalyzerConnector.Simulator.Communication.Tcp;

public sealed class AnalyzerTcpClient : IAnalyzerConnection
{
    private readonly string _host;

    private readonly int _port;

    private TcpClient? _client;

    private NetworkStream? _stream;

    private CancellationTokenSource? _cts;

    public bool IsConnected =>
        _client?.Connected == true;

    public event EventHandler<string>? DataReceived;

    public AnalyzerTcpClient(
        string host,
        int port)
    {
        _host = host;
        _port = port;
    }

    public async Task StartAsync()
    {
        if (IsConnected)
        {
            return;
        }

        _client = new TcpClient();

        await _client.ConnectAsync(
            _host,
            _port);

        _stream = _client.GetStream();

        _cts = new CancellationTokenSource();

        _ = ReceiveLoopAsync(_cts.Token);
    }

    private async Task ReceiveLoopAsync(
    CancellationToken cancellationToken)
    {
        if (_stream is null)
        {
            return;
        }

        byte[] buffer = new byte[4096];

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int bytesRead =
                    await _stream.ReadAsync(
                        buffer,
                        cancellationToken);

                if (bytesRead == 0)
                {
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
        catch
        {
            // Ignore disconnects.
        }
    }

    public async Task StopAsync()
    {
        try
        {
            _cts?.Cancel();

            if (_stream is not null)
            {
                await _stream.DisposeAsync();
                _stream = null;
            }

            _client?.Close();
            _client?.Dispose();

            _client = null;
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
        }
    }

    public async Task SendAsync(
      string message)
    {
        if (_stream is null)
        {
            throw new InvalidOperationException(
                "Simulator is not connected.");
        }

        byte[] bytes =
            Encoding.ASCII.GetBytes(
                message);

        await _stream.WriteAsync(
            bytes);

        await _stream.FlushAsync();
    }
}