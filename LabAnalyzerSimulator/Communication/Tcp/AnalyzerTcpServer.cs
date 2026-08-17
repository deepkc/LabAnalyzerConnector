using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LabAnalyzerConnector.Simulator.Communication.Tcp;

public sealed class AnalyzerTcpServer : IAnalyzerConnection
{
    private readonly int _port;

    private TcpListener? _listener;

    private TcpClient? _client;

    private NetworkStream? _stream;

    private CancellationTokenSource? _cts;

    public bool IsConnected =>
        _client?.Connected == true;

    public event EventHandler<string>? DataReceived;

    public AnalyzerTcpServer(int port)
    {
        _port = port;
    }

    public async Task StartAsync()
    {
        if (_listener != null)
        {
            return;
        }

        _cts = new CancellationTokenSource();

        _listener = new TcpListener(
            IPAddress.Any,
            _port);

        _listener.Start();

        _client =
            await _listener.AcceptTcpClientAsync(
                _cts.Token);

        _stream =
            _client.GetStream();

        _ = Task.Run(
            ReceiveLoopAsync);
    }

    private async Task ReceiveLoopAsync()
    {
        if (_stream == null)
        {
            return;
        }

        byte[] buffer = new byte[4096];

        try
        {
            while (_cts != null &&
                   !_cts.IsCancellationRequested)
            {
                int bytesRead =
                    await _stream.ReadAsync(
                        buffer,
                        _cts.Token);

                if (bytesRead == 0)
                {
                    break;
                }

                string message =
                    Encoding.ASCII.GetString(
                        buffer,
                        0,
                        bytesRead);

                DataReceived?.Invoke(
                    this,
                    message);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal during shutdown
        }
    }

    public Task StopAsync()
    {
        _cts?.Cancel();

        _stream?.Dispose();

        _client?.Close();

        _listener?.Stop();

        _stream = null;

        _client = null;

        _listener = null;

        return Task.CompletedTask;
    }

    public async Task SendAsync(string message)
    {
        if (_stream == null)
        {
            return;
        }

        byte[] bytes =
            Encoding.ASCII.GetBytes(message);

        await _stream.WriteAsync(bytes);

        await _stream.FlushAsync();
    }
}