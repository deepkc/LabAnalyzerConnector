using System.ComponentModel.DataAnnotations;

namespace LabAnalyzerConnector.Infrastructure.Persistence.Entities;

public sealed class AnalyzerConfigurationEntity
{
    [Key]
    public Guid AnalyzerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Manufacturer { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public string AnalyzerCode { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }

    public bool AutoConnect { get; set; }

    public int ConnectionType { get; set; }

    public int Direction { get; set; }

    public string ProtocolJson { get; set; } = string.Empty;

    public string? TcpJson { get; set; }

    public string? SerialJson { get; set; }

    public Guid? MappingProfileId { get; set; }

    public bool AutoReconnect { get; set; }

    public int ReconnectDelaySeconds { get; set; }

    public int MaxReconnectAttempts { get; set; }

    public bool EnableRawMessageLogging { get; set; }

    public bool EnableParsedMessageLogging { get; set; }

    public bool EnableErrorLogging { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}