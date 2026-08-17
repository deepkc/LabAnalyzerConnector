using System.Globalization;
using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Services;

public sealed class UnitConversionEngine
    : IUnitConversionEngine
{
    public decimal? Convert(
        Guid analyzerId,
        string testCode,
        decimal value,
        string sourceUnit,
        IEnumerable<UnitConversionRule> rules)
    {
        if (string.IsNullOrWhiteSpace(testCode))
        {
            return value;
        }

        if (string.IsNullOrWhiteSpace(sourceUnit))
        {
            return value;
        }

        UnitConversionRule? rule =
            rules
                .Where(x => x.IsActive)
                .FirstOrDefault(
                    x =>
                        x.AnalyzerId == analyzerId &&
                        string.Equals(
                            x.TestCode,
                            testCode,
                            StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(
                            x.SourceUnit,
                            sourceUnit,
                            StringComparison.OrdinalIgnoreCase));

        if (rule == null)
        {
            return value;
        }

        decimal convertedValue =
            (value * rule.Multiplier)
            + rule.Offset;

        return Math.Round(
            convertedValue,
            rule.DecimalPlaces);
    }
}