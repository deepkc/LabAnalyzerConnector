using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Abstractions;

public interface IResultTransformationEngine
{
    string? Transform(
        Guid analyzerId,
        string testCode,
        string? sourceValue,
        IEnumerable<ResultTransformation> transformations);
}