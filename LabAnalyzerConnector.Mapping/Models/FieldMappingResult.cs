namespace LabAnalyzerConnector.Mapping.Models;

public sealed class FieldMappingResult
{
    public Guid AnalyzerId { get; set; }

    public Dictionary<string, string?> Fields { get; set; }
        = new();

    public List<string> MissingRequiredFields { get; set; }
        = new();

    public bool IsValid =>
        MissingRequiredFields.Count == 0;
}