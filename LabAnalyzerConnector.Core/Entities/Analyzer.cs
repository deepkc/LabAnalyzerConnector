using LabAnalyzerConnector.Core.Enums;
using LabAnalyzerConnector.Core.Configuration;

namespace LabAnalyzerConnector.Core.Entities;

public class Analyzer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public AnalyzerConfiguration Configuration { get; set; } = new();

    public string Manufacturer { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string AnalyzerCode { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }

    public AnalyzerStatus Status { get; set; } = AnalyzerStatus.Stopped;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }
}