using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Core.Configuration;

public class ProtocolConfiguration
{
    // =========================================================
    // Protocol
    // =========================================================

    /// <summary>
    /// ASTM, HL7, etc.
    /// </summary>
    public ProtocolType ProtocolType { get; set; }

    /// <summary>
    /// Protocol version.
    /// Examples:
    /// ASTM 1394
    /// HL7 2.3
    /// HL7 2.5
    /// HL7 2.7
    /// </summary>
    public string ProtocolVersion { get; set; } = string.Empty;

    // =========================================================
    // Encoding
    // =========================================================

    /// <summary>
    /// Character encoding.
    /// ASCII, UTF8, UTF-16...
    /// </summary>
    public string EncodingName { get; set; } = "ASCII";

    // =========================================================
    // Acknowledgement
    // =========================================================

    /// <summary>
    /// Whether ACK is required.
    /// </summary>
    public bool RequireAcknowledgement { get; set; } = true;

    /// <summary>
    /// ACK timeout.
    /// </summary>
    public int AcknowledgementTimeoutMilliseconds { get; set; } = 5000;

    /// <summary>
    /// Retry count.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    // =========================================================
    // Frame Characters
    // =========================================================

    public byte StartOfText { get; set; } = 0x02;   // STX

    public byte EndOfText { get; set; } = 0x03;     // ETX

    public byte EndOfTransmission { get; set; } = 0x04; // EOT

    public byte Enquiry { get; set; } = 0x05;       // ENQ

    public byte Acknowledge { get; set; } = 0x06;   // ACK

    public byte NegativeAcknowledge { get; set; } = 0x15; // NAK

    public byte LineFeed { get; set; } = 0x0A;

    public byte CarriageReturn { get; set; } = 0x0D;

    // =========================================================
    // Timing
    // =========================================================

    public int ReceiveTimeoutMilliseconds { get; set; } = 30000;

    public int SendTimeoutMilliseconds { get; set; } = 30000;

    // =========================================================
    // Behaviour
    // =========================================================

    /// <summary>
    /// Validate checksum (ASTM).
    /// </summary>
    public bool ValidateChecksum { get; set; } = true;

    /// <summary>
    /// Ignore malformed frames and continue.
    /// </summary>
    public bool IgnoreInvalidFrames { get; set; } = false;

    /// <summary>
    /// Write raw protocol frames to the log.
    /// </summary>
    public bool LogRawFrames { get; set; } = true;
}