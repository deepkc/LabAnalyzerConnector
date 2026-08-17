using System.IO.Ports;

namespace LabAnalyzerConnector.Core.Configuration;

public class SerialConfiguration
{
    // =========================================================
    // Serial Port
    // =========================================================

    /// <summary>
    /// Serial/COM port name.
    /// Example: COM1, COM3, COM5.
    /// </summary>
    public string PortName { get; set; } = "COM1";


    // =========================================================
    // Communication Settings
    // =========================================================

    /// <summary>
    /// Communication speed.
    /// Common analyzer values: 9600, 19200, 38400, 57600, 115200.
    /// </summary>
    public int BaudRate { get; set; } = 9600;

    /// <summary>
    /// Number of data bits.
    /// Common values: 7 or 8.
    /// </summary>
    public int DataBits { get; set; } = 8;

    /// <summary>
    /// Parity checking mode.
    /// </summary>
    public Parity Parity { get; set; } =
        Parity.None;

    /// <summary>
    /// Number of stop bits.
    /// </summary>
    public StopBits StopBits { get; set; } =
        StopBits.One;

    /// <summary>
    /// Hardware/software flow control.
    /// </summary>
    public Handshake Handshake { get; set; } =
        Handshake.None;


    // =========================================================
    // Flow Control
    // =========================================================

    /// <summary>
    /// Controls the DTR (Data Terminal Ready) signal.
    /// </summary>
    public bool DtrEnable { get; set; } = false;

    /// <summary>
    /// Controls the RTS (Request To Send) signal.
    /// </summary>
    public bool RtsEnable { get; set; } = false;


    // =========================================================
    // Timeouts
    // =========================================================

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
    // Reconnection
    // =========================================================

    /// <summary>
    /// Automatically reconnect when the serial connection
    /// is unexpectedly lost.
    /// </summary>
    public bool AutoReconnect { get; set; } = true;

    /// <summary>
    /// Delay between reconnection attempts.
    /// </summary>
    public int ReconnectIntervalSeconds { get; set; } = 5;

    /// <summary>
    /// Maximum number of reconnection attempts.
    /// 0 = unlimited attempts.
    /// </summary>
    public int MaxReconnectAttempts { get; set; } = 0;
}