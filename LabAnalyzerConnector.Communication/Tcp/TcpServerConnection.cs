using System.Net;
using System.Net.Sockets;
using System.Text;
using LabAnalyzerConnector.Communication.Abstractions;
using LabAnalyzerConnector.Communication.Exceptions;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Communication.Tcp;

public sealed class TcpServerConnection : IAnalyzerConnection
{
    private readonly Guid _analyzerId;
    private readonly TcpConfiguration _configuration;

    private TcpListener? _tcpListener;
    private TcpClient? _connectedClient;
    private NetworkStream? _networkStream;

    private CancellationTokenSource? _serverCancellation;

    private readonly SemaphoreSlim _lifecycleLock =
        new(1, 1);

    private readonly SemaphoreSlim _sendLock =
        new(1, 1);

    public Guid AnalyzerId =>
        _analyzerId;

    public ConnectionStatus Status
    {
        get;
        private set;
    } =
        ConnectionStatus.Disconnected;

    public event EventHandler<ConnectionStatus>?
        StatusChanged;

    public event EventHandler<string>?
        DataReceived;

    public event EventHandler<Exception>?
        ErrorOccurred;
    public CommunicationDirection Direction
    {
        get;
    }

    // =========================================================
    // CONSTRUCTOR
    // =========================================================
    public TcpServerConnection(
        Guid analyzerId,
        TcpConfiguration configuration,
        CommunicationDirection direction)
    {
        _analyzerId =
            analyzerId;

        _configuration =
            configuration
            ?? throw new ArgumentNullException(
                nameof(configuration));

        Direction =
            direction;
    }



    // =========================================================
    // START TCP SERVER
    // =========================================================

    public async Task ConnectAsync(
        CancellationToken cancellationToken = default)
    {
        await _lifecycleLock.WaitAsync(
            cancellationToken);

        try
        {
            if (Status == ConnectionStatus.Connected ||
                Status == ConnectionStatus.Connecting)
            {
                return;
            }

            SetStatus(
                ConnectionStatus.Connecting);

            // -------------------------------------------------
            // Clean up previous server state
            // -------------------------------------------------

            _serverCancellation?.Dispose();

            _serverCancellation =
                CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken);

            // -------------------------------------------------
            // Resolve local IP
            // -------------------------------------------------

            IPAddress ipAddress;

            if (string.IsNullOrWhiteSpace(
                    _configuration.LocalIpAddress))
            {
                ipAddress =
                    IPAddress.Any;
            }
            else if (!IPAddress.TryParse(
                         _configuration.LocalIpAddress,
                         out ipAddress!))
            {
                ipAddress =
                    IPAddress.Any;
            }

            // -------------------------------------------------
            // Create TCP listener
            // -------------------------------------------------

            _tcpListener =
                new TcpListener(
                    ipAddress,
                    _configuration.LocalPort);

            // -------------------------------------------------
            // Start listening
            // -------------------------------------------------

            _tcpListener.Start();

            // -------------------------------------------------
            // Start accepting analyzer connection
            // -------------------------------------------------

            _ = AcceptClientLoopAsync(
                _serverCancellation.Token);

            // IMPORTANT:
            //
            // At this point the SERVER is listening.
            //
            // We do NOT report Connected yet.
            //
            // Connected means an analyzer/client has actually
            // connected to this TCP server.
            //
            // Therefore the status remains Connecting until
            // Compal connects.
            // -------------------------------------------------
        }
        catch (OperationCanceledException)
        {
            SetStatus(
                ConnectionStatus.Disconnected);

            throw;
        }
        catch (Exception ex)
        {
            SetStatus(
                ConnectionStatus.Error);

            var connectionException =
                new ConnectionException(
                    $"Failed to start TCP server on " +
                    $"{_configuration.LocalIpAddress}:" +
                    $"{_configuration.LocalPort}.",
                    ex);

            ErrorOccurred?.Invoke(
                this,
                connectionException);

            throw connectionException;
        }
        finally
        {
            _lifecycleLock.Release();
        }
    }


    // =========================================================
    // ACCEPT CLIENT LOOP
    // =========================================================

    private async Task AcceptClientLoopAsync(
        CancellationToken cancellationToken)
    {
        if (_tcpListener is null)
        {
            return;
        }

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient client =
                    await _tcpListener.AcceptTcpClientAsync(
                        cancellationToken);

                // -------------------------------------------------
                // If another client is already connected,
                // reject the new connection for now.
                //
                // This is intentional for our first test.
                //
                // Later we will redesign the server to support
                // multiple analyzers simultaneously.
                // -------------------------------------------------

                if (_connectedClient is not null)
                {
                    client.Close();
                    client.Dispose();

                    continue;
                }

                // -------------------------------------------------
                // Handle connected analyzer
                // -------------------------------------------------

                await HandleClientAsync(
                    client,
                    cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when server is intentionally stopped.
        }
        catch (ObjectDisposedException)
        {
            // Expected when listener is disposed.
        }
        catch (Exception ex)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                SetStatus(
                    ConnectionStatus.Error);

                ErrorOccurred?.Invoke(
                    this,
                    ex);
            }
        }
    }


    // =========================================================
    // HANDLE CONNECTED ANALYZER
    // =========================================================

    private async Task HandleClientAsync(
        TcpClient client,
        CancellationToken cancellationToken)
    {
        try
        {
            _connectedClient =
                client;

            _networkStream =
                client.GetStream();

            // -------------------------------------------------
            // Analyzer is now actually connected
            // -------------------------------------------------

            SetStatus(
                ConnectionStatus.Connected);

            byte[] buffer =
                new byte[8192];

            while (
                !cancellationToken.IsCancellationRequested)
            {

                Console.WriteLine(
       "!!!!!!!!!!!! TCP SERVER HandleClientAsync HIT !!!!!!!!!!!!");
                int bytesRead =
                    await _networkStream.ReadAsync(
                        buffer,
                        cancellationToken);

                // -------------------------------------------------
                // Remote side disconnected
                // -------------------------------------------------

                if (bytesRead == 0)
                {
                    break;
                }

                // -------------------------------------------------
                // Convert bytes to text
                //
                // Current default is ASCII.
                //
                // Later we will use the EncodingName
                // from ProtocolConfiguration.
                // -------------------------------------------------

                string data =
                    Encoding.ASCII.GetString(
                        buffer,
                        0,
                        bytesRead);

                Console.WriteLine(
    $"[TCP DEBUG] Bytes={bytesRead}, " +
    $"FirstByte=0x{buffer[0]:X2}, " +
    $"LastByte=0x{buffer[bytesRead - 1]:X2}");

                Console.WriteLine(
                    $"[TCP DEBUG] VT={buffer.Take(bytesRead).Contains((byte)0x0B)}, " +
                    $"FS={buffer.Take(bytesRead).Contains((byte)0x1C)}, " +
                    $"CR={buffer.Take(bytesRead).Contains((byte)0x0D)}");

                // -------------------------------------------------
                // Notify application
                // -------------------------------------------------

                // =========================================================
                // DIRECTION CHECK
                // =========================================================

                if (Direction == CommunicationDirection.Outbound)
                {
                    // Outbound analyzers are not expected
                    // to send data to the application.

                    System.Diagnostics.Debug.WriteLine(
                        $"[TCP SERVER] Incoming data ignored because " +
                        $"analyzer is configured as Outbound.");

                    continue;
                }


                // =========================================================
                // FOR INBOUND OR BIDIRECTIONAL
                // FORWARD DATA TO APPLICATION
                // =========================================================

                Console.WriteLine("========================================");
                Console.WriteLine("[TCP SERVER] RAW BYTE DEBUG");
                Console.WriteLine("========================================");

                Console.WriteLine(
                    $"BytesRead = {bytesRead}");

                Console.WriteLine(
                    $"First byte = {(bytesRead > 0 ? $"0x{buffer[0]:X2}" : "NONE")}");

                Console.WriteLine(
                    $"Last byte = {(bytesRead > 0 ? $"0x{buffer[bytesRead - 1]:X2}" : "NONE")}");

                Console.WriteLine(
                    $"Contains VT  = {Array.IndexOf(buffer, (byte)0x0B, 0, bytesRead) >= 0}");

                Console.WriteLine(
                    $"Contains FS  = {Array.IndexOf(buffer, (byte)0x1C, 0, bytesRead) >= 0}");

                Console.WriteLine(
                    $"Contains CR  = {Array.IndexOf(buffer, (byte)0x0D, 0, bytesRead) >= 0}");

                Console.WriteLine(
                    "HEX:");

                for (int i = 0; i < bytesRead; i++)
                {
                    Console.Write($"{buffer[i]:X2} ");
                }

                Console.WriteLine();
                Console.WriteLine("========================================");

                DataReceived?.Invoke(
                    this,
                    data);


                Console.WriteLine("[TCP SERVER] RECEIVED RAW:");

                foreach (char c in data)
                {
                    Console.Write(
                        c switch
                        {
                            '\u000B' => "<VT>",
                            '\u001C' => "<FS>",
                            '\r' => "<CR>",
                            '\n' => "<LF>",
                            '\u0005' => "<ENQ>",
                            '\u0006' => "<ACK>",
                            _ => c.ToString()
                        });
                }

                Console.WriteLine();

                Console.WriteLine("[TCP SERVER] RECEIVED RAW:");

                foreach (char c in data)
                {
                    Console.Write(
                        c switch
                        {
                            '\u000B' => "<VT>",
                            '\u001C' => "<FS>",
                            '\r' => "<CR>",
                            '\n' => "<LF>",
                            '\u0005' => "<ENQ>",
                            '\u0006' => "<ACK>",
                            _ => c.ToString()
                        });
                }

                Console.WriteLine();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when server is intentionally stopped.
        }
        catch (ObjectDisposedException)
        {
            // Expected during shutdown.
        }
        catch (Exception ex)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                SetStatus(
                    ConnectionStatus.Error);

                ErrorOccurred?.Invoke(
                    this,
                    ex);
            }
        }
        finally
        {
            await CloseClientAsync();

            if (!cancellationToken.IsCancellationRequested &&
                Status != ConnectionStatus.Error)
            {
                SetStatus(
                    ConnectionStatus.Disconnected);
            }
        }
    }


    // =========================================================
    // SEND DATA TO CONNECTED ANALYZER
    // =========================================================

    public async Task SendAsync(
     string data,
     CancellationToken cancellationToken = default)
    {
        // =========================================================
        // DIRECTION CHECK
        // =========================================================

        if (Direction == CommunicationDirection.Inbound)
        {
            throw new ConnectionException(
                "Cannot send data because this analyzer " +
                "is configured for Inbound communication only.");
        }


        // =========================================================
        // VALIDATE DATA
        // =========================================================

        if (string.IsNullOrEmpty(data))
        {
            throw new ArgumentException(
                "Data cannot be empty.",
                nameof(data));
        }


        // =========================================================
        // GET ACTIVE CONNECTION
        // =========================================================

        NetworkStream? networkStream =
            _networkStream;

        TcpClient? client =
            _connectedClient;


        if (networkStream is null ||
            client is null ||
            !client.Connected)
        {
            throw new ConnectionException(
                "No analyzer is currently connected " +
                "to the TCP server.");
        }


        // =========================================================
        // SEND LOCK
        // =========================================================

        await _sendLock.WaitAsync(
            cancellationToken);

        try
        {
            byte[] bytes =
                Encoding.ASCII.GetBytes(
                    data);


            await networkStream.WriteAsync(
                bytes,
                cancellationToken);


            await networkStream.FlushAsync(
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                SetStatus(
                    ConnectionStatus.Error);

                ErrorOccurred?.Invoke(
                    this,
                    ex);
            }

            throw new ConnectionException(
                "Failed to send data to the connected analyzer.",
                ex);
        }
        finally
        {
            _sendLock.Release();
        }
    }


    // =========================================================
    // STOP TCP SERVER
    // =========================================================

    public async Task DisconnectAsync()
    {
        await _lifecycleLock.WaitAsync();

        try
        {
            // -------------------------------------------------
            // Cancel server operations
            // -------------------------------------------------

            _serverCancellation?.Cancel();

            // -------------------------------------------------
            // Stop TCP listener
            // -------------------------------------------------

            _tcpListener?.Stop();

            _tcpListener =
                null;

            // -------------------------------------------------
            // Close connected analyzer
            // -------------------------------------------------

            await CloseClientAsync();

            // -------------------------------------------------
            // Update status
            // -------------------------------------------------

            SetStatus(
                ConnectionStatus.Disconnected);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(
                this,
                ex);

            SetStatus(
                ConnectionStatus.Error);
        }
        finally
        {
            _serverCancellation?.Dispose();

            _serverCancellation =
                null;

            _lifecycleLock.Release();
        }
    }


    // =========================================================
    // CLOSE CONNECTED CLIENT
    // =========================================================

    private async Task CloseClientAsync()
    {
        NetworkStream? stream =
            _networkStream;

        TcpClient? client =
            _connectedClient;

        _networkStream =
            null;

        _connectedClient =
            null;

        if (stream is not null)
        {
            try
            {
                await stream.DisposeAsync();
            }
            catch
            {
                // Ignore cleanup errors.
            }
        }

        if (client is not null)
        {
            try
            {
                client.Close();

                client.Dispose();
            }
            catch
            {
                // Ignore cleanup errors.
            }
        }
    }


    // =========================================================
    // STATUS MANAGEMENT
    // =========================================================

    private void SetStatus(
        ConnectionStatus status)
    {
        if (Status == status)
        {
            return;
        }

        Status =
            status;

        StatusChanged?.Invoke(
            this,
            status);
    }


    // =========================================================
    // DISPOSE
    // =========================================================

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();

        _serverCancellation?.Dispose();

        _lifecycleLock.Dispose();

        _sendLock.Dispose();
    }
}