using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Services;

public sealed class FieldMappingEngine
    : IFieldMappingEngine
{
    public FieldMappingResult MapFields(
        Guid analyzerId,
        IEnumerable<FieldMapping> mappings,
        IReadOnlyDictionary<string, string?> sourceFields)
    {
        var result =
            new FieldMappingResult
            {
                AnalyzerId = analyzerId
            };

        foreach (FieldMapping mapping in mappings)
        {
            if (!mapping.IsActive)
            {
                continue;
            }

            sourceFields.TryGetValue(
                mapping.SourceField,
                out string? value);

            result.Fields[
                mapping.TargetField] = value;

            if (mapping.IsRequired &&
                string.IsNullOrWhiteSpace(value))
            {
                result.MissingRequiredFields.Add(
                    mapping.TargetField);
            }
        }

        return result;
    }
}