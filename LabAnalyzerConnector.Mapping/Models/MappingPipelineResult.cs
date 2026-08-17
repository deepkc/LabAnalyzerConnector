using LabAnalyzerConnector.Domain.Entities;

namespace LabAnalyzerConnector.Mapping.Models;

public sealed class MappingPipelineResult
{
    public bool IsSuccess { get; set; }

    public LabResult? Result { get; set; }

    public List<string> Errors { get; set; } = new();

    public List<string> Warnings { get; set; } = new();
}