using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Core.Configuration;

public class AnalyzerConfiguration
{
    // =========================================================
    // Identity
    // =========================================================

    /// <summary>
    /// Unique ID of this configuration record.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Unique identifier of the physical analyzer.
    /// </summary>
    public Guid AnalyzerId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// User-friendly analyzer name.
    /// Example: Nihon Kohden MEK-9100 #1
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Analyzer manufacturer.
    /// Example: Nihon Kohden
    /// </summary>
    public string Manufacturer { get; set; } = string.Empty;

    /// <summary>
    /// Analyzer model.
    /// Example: MEK-9100
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Optional serial number of the physical analyzer.
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Optional laboratory-assigned analyzer code.
    /// </summary>
    public string AnalyzerCode { get; set; } = string.Empty;


    // =========================================================
    // Status
    // =========================================================

    /// <summary>
    /// Determines whether this analyzer configuration is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Determines whether the analyzer should automatically
    /// connect when the application starts.
    /// </summary>
    public bool AutoConnect { get; set; } = true;


    // =========================================================
    // Communication
    // =========================================================

    /// <summary>
    /// Communication method used by the analyzer.
    /// TCP/IP or Serial/RS-232.
    /// </summary>
    public ConnectionType ConnectionType { get; set; }

    /// <summary>
    /// Direction of communication.
    /// Inbound, Outbound or Bidirectional.
    /// </summary>
    public CommunicationDirection Direction { get; set; } =
        CommunicationDirection.Bidirectional;


    // =========================================================
    // Protocol
    // =========================================================

    /// <summary>
    /// Protocol configuration for this analyzer.
    /// Examples: ASTM or HL7.
    /// </summary>
    public ProtocolConfiguration Protocol { get; set; } = new();


    // =========================================================
    // TCP/IP Configuration
    // =========================================================

    /// <summary>
    /// TCP/IP configuration.
    /// Required when ConnectionType is TCP/IP.
    /// </summary>
    public TcpConfiguration? Tcp { get; set; }


    // =========================================================
    // Serial / RS-232 Configuration
    // =========================================================

    /// <summary>
    /// Serial configuration.
    /// Required when ConnectionType is Serial.
    /// </summary>
    public SerialConfiguration? Serial { get; set; }


    // =========================================================
    // Mapping
    // =========================================================

    /// <summary>
    /// ID of the analyzer-specific mapping profile.
    /// </summary>
    public Guid? MappingProfileId { get; set; }


    // =========================================================
    // Connection Behaviour
    // =========================================================

    /// <summary>
    /// Automatically reconnect when the connection is lost.
    /// </summary>
    public bool AutoReconnect { get; set; } = true;

    /// <summary>
    /// Delay before attempting to reconnect, in seconds.
    /// </summary>
    public int ReconnectDelaySeconds { get; set; } = 5;

    /// <summary>
    /// Maximum number of reconnection attempts.
    /// Use -1 for unlimited attempts.
    /// </summary>
    public int MaxReconnectAttempts { get; set; } = -1;


    // =========================================================
    // Message Handling
    // =========================================================

    /// <summary>
    /// Whether incoming raw messages should be logged.
    /// </summary>
    public bool EnableRawMessageLogging { get; set; } = true;

    /// <summary>
    /// Whether parsed messages should be logged.
    /// </summary>
    public bool EnableParsedMessageLogging { get; set; } = true;

    /// <summary>
    /// Whether communication errors should be logged.
    /// </summary>
    public bool EnableErrorLogging { get; set; } = true;


    // =========================================================
    // Audit Information
    // =========================================================

    /// <summary>
    /// Date and time when the configuration was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } =
        DateTime.UtcNow;

    /// <summary>
    /// Date and time when the configuration was last modified.
    /// </summary>
    public DateTime UpdatedAtUtc { get; set; } =
        DateTime.UtcNow;
}