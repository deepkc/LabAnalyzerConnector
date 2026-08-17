using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Services;

public sealed class ResultTransformationEngine
    : IResultTransformationEngine
{
    public string? Transform(
        Guid analyzerId,
        string testCode,
        string? sourceValue,
        IEnumerable<ResultTransformation> transformations)
    {
        if (string.IsNullOrWhiteSpace(sourceValue))
        {
            return sourceValue;
        }

        if (string.IsNullOrWhiteSpace(testCode))
        {
            return sourceValue;
        }

        ResultTransformation? transformation =
            transformations
                .Where(x => x.IsActive)
                .FirstOrDefault(
                    x =>
                        x.AnalyzerId == analyzerId &&
                        string.Equals(
                            x.TestCode,
                            testCode,
                            StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(
                            x.SourceValue,
                            sourceValue,
                            StringComparison.OrdinalIgnoreCase));

        return transformation?.TargetValue
               ?? sourceValue;
    }
}