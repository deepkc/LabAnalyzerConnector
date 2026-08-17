using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Abstractions;

public interface IUnitConversionEngine
{
    decimal? Convert(
        Guid analyzerId,
        string testCode,
        decimal value,
        string sourceUnit,
        IEnumerable<UnitConversionRule> rules);
}