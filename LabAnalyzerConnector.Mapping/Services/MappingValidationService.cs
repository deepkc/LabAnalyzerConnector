using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Services;

public sealed class MappingValidationService
    : IMappingValidationService
{
    public MappingValidationResult Validate(
        AnalyzerMappingProfile profile)
    {
        var result = new MappingValidationResult();

        ValidateProfile(profile, result);

        ValidateTestCodeMappings(
            profile,
            result);

        ValidateFieldMappings(
            profile,
            result);

        ValidateTransformations(
            profile,
            result);

        ValidateUnitConversionRules(
    profile,
    result);

        return result;
    }

    private static void ValidateProfile(
        AnalyzerMappingProfile profile,
        MappingValidationResult result)
    {
        if (profile.AnalyzerId == Guid.Empty)
        {
            result.Errors.Add(
                "Analyzer ID is required.");
        }

        if (string.IsNullOrWhiteSpace(
                profile.AnalyzerName))
        {
            result.Errors.Add(
                "Analyzer name is required.");
        }
    }

    private static void ValidateUnitConversionRules(
    AnalyzerMappingProfile profile,
    MappingValidationResult result)
    {
        var activeRules =
            profile.UnitConversionRules
                .Where(x => x.IsActive)
                .ToList();

        foreach (var rule in activeRules)
        {
            if (string.IsNullOrWhiteSpace(
                    rule.TestCode))
            {
                result.Errors.Add(
                    "Unit conversion test code cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(
                    rule.SourceUnit))
            {
                result.Errors.Add(
                    $"Source unit is missing for test '{rule.TestCode}'.");
            }

            if (string.IsNullOrWhiteSpace(
                    rule.TargetUnit))
            {
                result.Errors.Add(
                    $"Target unit is missing for test '{rule.TestCode}'.");
            }

            if (rule.DecimalPlaces < 0)
            {
                result.Errors.Add(
                    $"Decimal places cannot be negative for test '{rule.TestCode}'.");
            }

            if (rule.AnalyzerId !=
                profile.AnalyzerId)
            {
                result.Errors.Add(
                    $"Unit conversion rule for test '{rule.TestCode}' belongs to a different analyzer.");
            }
        }

        var duplicateRules =
            activeRules
                .GroupBy(
                    x =>
                        $"{x.TestCode}|{x.SourceUnit}|{x.TargetUnit}",
                    StringComparer.OrdinalIgnoreCase)
                .Where(
                    x => x.Count() > 1);

        foreach (var duplicate in duplicateRules)
        {
            result.Errors.Add(
                $"Duplicate unit conversion rule found for '{duplicate.Key}'.");
        }
    }
    private static void ValidateTestCodeMappings(
        AnalyzerMappingProfile profile,
        MappingValidationResult result)
    {
        var activeMappings =
            profile.TestCodeMappings
                .Where(x => x.IsActive)
                .ToList();

        foreach (var mapping in activeMappings)
        {
            if (string.IsNullOrWhiteSpace(
                    mapping.AnalyzerTestCode))
            {
                result.Errors.Add(
                    "Analyzer test code cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(
                    mapping.StandardTestCode))
            {
                result.Errors.Add(
                    $"Standard test code is missing for analyzer code '{mapping.AnalyzerTestCode}'.");
            }

            if (mapping.AnalyzerId !=
                profile.AnalyzerId)
            {
                result.Errors.Add(
                    $"Test code mapping '{mapping.AnalyzerTestCode}' belongs to a different analyzer.");
            }
        }

        var duplicateCodes =
            activeMappings
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.AnalyzerTestCode))
                .GroupBy(
                    x => x.AnalyzerTestCode,
                    StringComparer.OrdinalIgnoreCase)
                .Where(
                    x => x.Count() > 1);

        foreach (var duplicate in duplicateCodes)
        {
            result.Errors.Add(
                $"Duplicate analyzer test code mapping found: '{duplicate.Key}'.");
        }
    }

    private static void ValidateFieldMappings(
        AnalyzerMappingProfile profile,
        MappingValidationResult result)
    {
        var activeMappings =
            profile.FieldMappings
                .Where(x => x.IsActive)
                .ToList();

        foreach (var mapping in activeMappings)
        {
            if (string.IsNullOrWhiteSpace(
                    mapping.SourceField))
            {
                result.Errors.Add(
                    "Field mapping source field cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(
                    mapping.TargetField))
            {
                result.Errors.Add(
                    "Field mapping target field cannot be empty.");
            }

            if (mapping.AnalyzerId !=
                profile.AnalyzerId)
            {
                result.Errors.Add(
                    $"Field mapping '{mapping.SourceField}' belongs to a different analyzer.");
            }
        }

        var duplicateMappings =
            activeMappings
                .GroupBy(
                    x =>
                        $"{x.MessageType}|{x.SourceField}",
                    StringComparer.OrdinalIgnoreCase)
                .Where(
                    x => x.Count() > 1);

        foreach (var duplicate in duplicateMappings)
        {
            result.Errors.Add(
                $"Duplicate field mapping found for '{duplicate.Key}'.");
        }
    }

    private static void ValidateTransformations(
        AnalyzerMappingProfile profile,
        MappingValidationResult result)
    {
        var activeTransformations =
            profile.ResultTransformations
                .Where(x => x.IsActive)
                .ToList();

        foreach (var transformation
            in activeTransformations)
        {
            if (string.IsNullOrWhiteSpace(
                    transformation.TestCode))
            {
                result.Errors.Add(
                    "Transformation test code cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(
                    transformation.SourceValue))
            {
                result.Errors.Add(
                    "Transformation source value cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(
                    transformation.TargetValue))
            {
                result.Errors.Add(
                    "Transformation target value cannot be empty.");
            }

            if (transformation.AnalyzerId !=
                profile.AnalyzerId)
            {
                result.Errors.Add(
                    $"Transformation for test '{transformation.TestCode}' belongs to a different analyzer.");
            }
        }

        var duplicateTransformations =
            activeTransformations
                .GroupBy(
                    x =>
                        $"{x.TestCode}|{x.SourceValue}",
                    StringComparer.OrdinalIgnoreCase)
                .Where(
                    x => x.Count() > 1);

        foreach (var duplicate
            in duplicateTransformations)
        {
            result.Errors.Add(
                $"Duplicate transformation found for '{duplicate.Key}'.");
        }
    }
}