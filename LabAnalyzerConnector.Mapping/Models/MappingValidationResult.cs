namespace LabAnalyzerConnector.Mapping.Models;

public sealed class MappingValidationResult
{
    public bool IsValid =>
        Errors.Count == 0;

    public List<string> Errors { get; set; } = new();

    public List<string> Warnings { get; set; } = new();
}