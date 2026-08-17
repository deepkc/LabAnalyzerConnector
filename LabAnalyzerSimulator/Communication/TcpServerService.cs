using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LabAnalyzerSimulator.Communication;

public sealed class TcpServerService : IDisposable
{
    private TcpListener? _listener;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private CancellationTokenSource? _cts;

    public bool IsRunning =>
        _listener is not null;

    public bool ClientConnected =>
        _client is not null &&
        _client.Connected;

    public event Action<string>? DataReceived;

    public event Action<string>? StatusChanged;

    public async Task StartAsync(
        int port)
    {
        if (IsRunning)
        {
            return;
        }

        _cts = new CancellationTokenSource();

        _listener =
            new TcpListener(
                IPAddress.Any,
                port);

        _listener.Start();

        StatusChanged?.Invoke(
            $"Listening on port {port}");

        _client =
            await _listener.AcceptTcpClientAsync(
                _cts.Token);

        _stream =
            _client.GetStream();

        StatusChanged?.Invoke(
            "Analyzer connected.");

        _ = ReceiveLoopAsync(
            _cts.Token);
    }

    private async Task ReceiveLoopAsync(
        CancellationToken token)
    {
        if (_stream is null)
        {
            return;
        }

        byte[] buffer =
            new byte[4096];

        while (!token.IsCancellationRequested)
        {
            int read =
                await _stream.ReadAsync(
                    buffer,
                    token);

            if (read <= 0)
            {
                break;
            }

            string message =
                Encoding.ASCII.GetString(
                    buffer,
                    0,
                    read);

            DataReceived?.Invoke(
                message);
        }

        StatusChanged?.Invoke(
            "Client disconnected.");
    }

    public async Task SendAsync(
        string message)
    {
        if (_stream is null)
        {
            return;
        }

        byte[] bytes =
            Encoding.ASCII.GetBytes(
                message);

        await _stream.WriteAsync(
            bytes);

        await _stream.FlushAsync();
    }

    public void Stop()
    {
        _cts?.Cancel();

        _stream?.Dispose();

        _client?.Close();

        _listener?.Stop();

        _stream = null;
        _client = null;
        _listener = null;

        StatusChanged?.Invoke(
            "Server stopped.");
    }

    public void Dispose()
    {
        Stop();
    }
}