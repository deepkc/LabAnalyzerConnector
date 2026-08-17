using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Abstractions;

public interface IFieldMappingEngine
{
    FieldMappingResult MapFields(
        Guid analyzerId,
        IEnumerable<FieldMapping> mappings,
        IReadOnlyDictionary<string, string?> sourceFields);
}