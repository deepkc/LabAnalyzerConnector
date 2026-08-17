namespace LabAnalyzerConnector.Mapping.Models;

public sealed class AnalyzerMappingProfile
{
    public Guid AnalyzerId { get; set; }

    public string AnalyzerName { get; set; } = string.Empty;

    public List<TestCodeMapping> TestCodeMappings { get; set; } = new();

    public List<FieldMapping> FieldMappings { get; set; } = new();

    public List<ResultTransformation> ResultTransformations { get; set; } = new();

    public List<UnitConversionRule> UnitConversionRules { get; set; } = new();
}