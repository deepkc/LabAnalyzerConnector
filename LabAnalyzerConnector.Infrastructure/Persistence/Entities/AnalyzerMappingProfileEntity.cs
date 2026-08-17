namespace LabAnalyzerConnector.Infrastructure.Persistence.Entities;

public sealed class AnalyzerMappingProfileEntity
{
    public Guid Id { get; set; }

    public Guid AnalyzerId { get; set; }

    public string AnalyzerName { get; set; } = string.Empty;
}