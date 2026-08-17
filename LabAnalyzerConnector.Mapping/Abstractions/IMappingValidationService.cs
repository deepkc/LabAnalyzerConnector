using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Abstractions;

public interface IMappingValidationService
{
    MappingValidationResult Validate(
        AnalyzerMappingProfile profile);
}