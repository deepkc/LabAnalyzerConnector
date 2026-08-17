using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Core.Configuration;

public class TcpConfiguration
{
    // =========================================================
    // Connection Mode
    // =========================================================

    /// <summary>
    /// Client: Application connects to the analyzer.
    /// Server: Analyzer connects to the application.
    /// </summary>
    public ConnectionMode Mode { get; set; } =
        ConnectionMode.Client;


    // =========================================================
    // Remote Endpoint
    // =========================================================

    /// <summary>
    /// Remote analyzer IP address.
    /// Used primarily in Client mode.
    /// </summary>
    public string RemoteIpAddress { get; set; } =
        string.Empty;

    /// <summary>
    /// Remote analyzer TCP port.
    /// Used primarily in Client mode.
    /// </summary>
    public int RemotePort { get; set; }


    // =========================================================
    // Local Endpoint
    // =========================================================

    /// <summary>
    /// Local IP address/interface to bind to.
    /// Use 0.0.0.0 to listen on all available interfaces.
    /// Used primarily in Server mode.
    /// </summary>
    public string LocalIpAddress { get; set; } =
        "0.0.0.0";

    /// <summary>
    /// Local TCP port on which the application listens.
    /// Used primarily in Server mode.
    /// </summary>
    public int LocalPort { get; set; }


    // =========================================================
    // Connection Timeouts
    // =========================================================

    /// <summary>
    /// Maximum time allowed to establish a connection.
    /// </summary>
    public int ConnectionTimeoutMilliseconds { get; set; } =
        10000;

    /// <summary>
    /// Maximum time to wait for incoming data.
    /// </summary>
    public int ReadTimeoutMilliseconds { get; set; } =
        30000;

    /// <summary>
    /// Maximum time to wait when sending data.
    /// </summary>
    public int WriteTimeoutMilliseconds { get; set; } =
        30000;


    // =========================================================
    // Keep Alive
    // =========================================================

    /// <summary>
    /// Enables TCP keep-alive.
    /// Helps detect broken network connections.
    /// </summary>
    public bool KeepAliveEnabled { get; set; } =
        true;

    /// <summary>
    /// Time before TCP keep-alive starts detecting
    /// an inactive connection.
    /// </summary>
    public int KeepAliveIdleSeconds { get; set; } =
        60;

    /// <summary>
    /// Interval between TCP keep-alive probes.
    /// </summary>
    public int KeepAliveIntervalSeconds { get; set; } =
        10;


    // =========================================================
    // Automatic Reconnection
    // =========================================================

    /// <summary>
    /// Automatically reconnect after an unexpected
    /// connection failure.
    /// </summary>
    public bool AutoReconnect { get; set; } =
        true;

    /// <summary>
    /// Delay between reconnection attempts.
    /// </summary>
    public int ReconnectIntervalSeconds { get; set; } =
        5;

    /// <summary>
    /// Maximum number of reconnection attempts.
    /// 0 = unlimited attempts.
    /// </summary>
    public int MaxReconnectAttempts { get; set; } =
        0;


    // =========================================================
    // Server Configuration
    // =========================================================

    /// <summary>
    /// Maximum number of simultaneous analyzer connections
    /// accepted by the TCP server.
    /// </summary>
    public int MaxConnections { get; set; } =
        10;


    // =========================================================
    // Connection Behaviour
    // =========================================================

    /// <summary>
    /// Whether the TCP connection should remain open
    /// after a message is processed.
    /// </summary>
    public bool PersistentConnection { get; set; } =
        true;
}