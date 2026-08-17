using System.Collections.Concurrent;
using LabAnalyzerConnector.Communication.Abstractions;
using LabAnalyzerConnector.Communication.Factories;
using LabAnalyzerConnector.Communication.Managers.Events;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Enums;
using LabAnalyzerConnector.Core.Abstractions;

namespace LabAnalyzerConnector.Communication.Managers;



public sealed class ConnectionManager :
    IAnalyzerConnectionManager,
    IAsyncDisposable
{
    private readonly ConnectionFactory _connectionFactory;

    private readonly ConcurrentDictionary<
        Guid,
        IAnalyzerConnection> _connections = new();

    public IReadOnlyDictionary<
        Guid,
        IAnalyzerConnection> Connections =>
        _connections;

    public event EventHandler<ConnectionStatusChangedEventArgs>?
        ConnectionStatusChanged;

    public event EventHandler<AnalyzerDataReceivedEventArgs>?
        DataReceived;

    public event EventHandler<AnalyzerErrorEventArgs>?
        ErrorOccurred;


    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public ConnectionManager(
        ConnectionFactory connectionFactory)
    {
        _connectionFactory =
            connectionFactory;
    }


    // =========================================================
    // ADD AND CONNECT
    // =========================================================

    public async Task AddAndConnectAsync(
        Guid analyzerId,
        AnalyzerConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        // -----------------------------------------------------
        // Prevent duplicate connection objects
        // -----------------------------------------------------

        if (_connections.ContainsKey(analyzerId))
        {
            throw new InvalidOperationException(
                $"A connection already exists for analyzer {analyzerId}.");
        }


        // -----------------------------------------------------
        // Create connection
        // -----------------------------------------------------

        IAnalyzerConnection connection =
            _connectionFactory.Create(
                analyzerId,
                configuration);


        // -----------------------------------------------------
        // Subscribe to events
        // -----------------------------------------------------

        SubscribeToConnectionEvents(
            connection);


        // -----------------------------------------------------
        // Register connection
        // -----------------------------------------------------

        if (!_connections.TryAdd(
                analyzerId,
                connection))
        {
            UnsubscribeFromConnectionEvents(
                connection);

            await connection.DisposeAsync();

            throw new InvalidOperationException(
                $"Failed to register connection for analyzer {analyzerId}.");
        }


        try
        {
            // -------------------------------------------------
            // Establish actual connection
            // -------------------------------------------------

            await connection.ConnectAsync(
                cancellationToken);
        }
        catch
        {
            // -------------------------------------------------
            // Connection failed
            //
            // Remove failed connection from manager
            // -------------------------------------------------

            _connections.TryRemove(
                analyzerId,
                out _);

            UnsubscribeFromConnectionEvents(
                connection);

            await connection.DisposeAsync();

            throw;
        }
    }


    // =========================================================
    // DISCONNECT
    // =========================================================

    public async Task DisconnectAsync(
        Guid analyzerId)
    {
        if (!_connections.TryGetValue(
                analyzerId,
                out IAnalyzerConnection? connection))
        {
            return;
        }

        await connection.DisconnectAsync();
    }


    // =========================================================
    // REMOVE CONNECTION
    // =========================================================

    public async Task RemoveAsync(
        Guid analyzerId)
    {
        if (!_connections.TryRemove(
                analyzerId,
                out IAnalyzerConnection? connection))
        {
            return;
        }


        // -----------------------------------------------------
        // Unsubscribe events
        // -----------------------------------------------------

        UnsubscribeFromConnectionEvents(
            connection);


        // -----------------------------------------------------
        // Dispose connection
        // -----------------------------------------------------

        await connection.DisposeAsync();
    }


    // =========================================================
    // SEND DATA
    // =========================================================

    public async Task SendAsync(
        Guid analyzerId,
        string data,
        CancellationToken cancellationToken = default)
    {
        if (!_connections.TryGetValue(
                analyzerId,
                out IAnalyzerConnection? connection))
        {
            throw new InvalidOperationException(
                $"No connection found for analyzer {analyzerId}.");
        }

        await connection.SendAsync(
            data,
            cancellationToken);
    }


    // =========================================================
    // GET CONNECTION
    // =========================================================

    public bool TryGetConnection(
        Guid analyzerId,
        out IAnalyzerConnection? connection)
    {
        return _connections.TryGetValue(
            analyzerId,
            out connection);
    }


    // =========================================================
    // SUBSCRIBE EVENTS
    // =========================================================

    private void SubscribeToConnectionEvents(
        IAnalyzerConnection connection)
    {
        connection.StatusChanged +=
            OnConnectionStatusChanged;

        connection.DataReceived +=
            OnDataReceived;

        connection.ErrorOccurred +=
            OnErrorOccurred;
    }


    // =========================================================
    // UNSUBSCRIBE EVENTS
    // =========================================================

    private void UnsubscribeFromConnectionEvents(
        IAnalyzerConnection connection)
    {
        connection.StatusChanged -=
            OnConnectionStatusChanged;

        connection.DataReceived -=
            OnDataReceived;

        connection.ErrorOccurred -=
            OnErrorOccurred;
    }


    // =========================================================
    // CONNECTION STATUS EVENT
    // =========================================================

    private void OnConnectionStatusChanged(
        object? sender,
        ConnectionStatus status)
    {
        if (sender is not IAnalyzerConnection connection)
        {
            return;
        }

        ConnectionStatusChanged?.Invoke(
            this,
            new ConnectionStatusChangedEventArgs(
                connection.AnalyzerId,
                status));
    }


    // =========================================================
    // DATA RECEIVED EVENT
    // =========================================================

    private void OnDataReceived(
     object? sender,
     string data)
    {
        if (sender is not IAnalyzerConnection connection)
        {
            Console.WriteLine(
                "!!!!!!!! DATA RECEIVED FROM UNKNOWN SENDER !!!!!!!!");

            return;
        }

        Console.WriteLine(
            "========================================");

        Console.WriteLine(
            "[CONNECTION MANAGER] DATA RECEIVED");

        Console.WriteLine(
            $"AnalyzerId    : {connection.AnalyzerId}");

        Console.WriteLine(
            $"Connection    : {connection.GetType().FullName}");

        Console.WriteLine(
            $"Data Length   : {data.Length}");

        Console.WriteLine(
            $"First Char    : {(data.Length > 0 ? $"0x{(int)data[0]:X2}" : "EMPTY")}");

        Console.WriteLine(
            $"Contains VT   : {data.Contains((char)0x0B)}");

        Console.WriteLine(
            $"Contains FS   : {data.Contains((char)0x1C)}");

        Console.WriteLine(
            "========================================");

        DataReceived?.Invoke(
            this,
            new AnalyzerDataReceivedEventArgs(
                connection.AnalyzerId,
                data));
    }


    // =========================================================
    // ERROR EVENT
    // =========================================================

    private void OnErrorOccurred(
        object? sender,
        Exception exception)
    {
        if (sender is not IAnalyzerConnection connection)
        {
            return;
        }

        ErrorOccurred?.Invoke(
            this,
            new AnalyzerErrorEventArgs(
                connection.AnalyzerId,
                exception));
    }


    // =========================================================
    // DISPOSE
    // =========================================================

    public async ValueTask DisposeAsync()
    {
        foreach (
            IAnalyzerConnection connection
            in _connections.Values)
        {
            UnsubscribeFromConnectionEvents(
                connection);

            await connection.DisposeAsync();
        }

        _connections.Clear();
    }
}