using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Services;

public sealed class AnalyzerMappingProfileService
    : IAnalyzerMappingProfileService
{
    private readonly Dictionary<Guid, AnalyzerMappingProfile>
        _profiles = new();

    public AnalyzerMappingProfile CreateProfile(
        Guid analyzerId,
        string analyzerName)
    {
        if (analyzerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Analyzer ID cannot be empty.",
                nameof(analyzerId));
        }

        if (string.IsNullOrWhiteSpace(analyzerName))
        {
            throw new ArgumentException(
                "Analyzer name is required.",
                nameof(analyzerName));
        }

        if (_profiles.ContainsKey(analyzerId))
        {
            throw new InvalidOperationException(
                $"A mapping profile already exists for analyzer '{analyzerId}'.");
        }

        var profile = new AnalyzerMappingProfile
        {
            AnalyzerId = analyzerId,
            AnalyzerName = analyzerName
        };

        _profiles.Add(analyzerId, profile);

        return profile;
    }

    public AnalyzerMappingProfile? GetProfile(
        Guid analyzerId)
    {
        _profiles.TryGetValue(
            analyzerId,
            out AnalyzerMappingProfile? profile);

        return profile;
    }

    public void AddTestCodeMapping(
        Guid analyzerId,
        TestCodeMapping mapping)
    {
        var profile = GetRequiredProfile(analyzerId);

        mapping.AnalyzerId = analyzerId;

        profile.TestCodeMappings.Add(mapping);
    }

    public void AddFieldMapping(
        Guid analyzerId,
        FieldMapping mapping)
    {
        var profile = GetRequiredProfile(analyzerId);

        mapping.AnalyzerId = analyzerId;

        profile.FieldMappings.Add(mapping);
    }

    public void AddResultTransformation(
        Guid analyzerId,
        ResultTransformation transformation)
    {
        var profile = GetRequiredProfile(analyzerId);

        transformation.AnalyzerId = analyzerId;

        profile.ResultTransformations.Add(
            transformation);
    }

    public bool RemoveTestCodeMapping(
        Guid analyzerId,
        Guid mappingId)
    {
        var profile = GetRequiredProfile(analyzerId);

        var mapping =
            profile.TestCodeMappings
                .FirstOrDefault(
                    x => x.Id == mappingId);

        if (mapping == null)
        {
            return false;
        }

        profile.TestCodeMappings.Remove(mapping);

        return true;
    }

    public bool RemoveFieldMapping(
        Guid analyzerId,
        Guid mappingId)
    {
        var profile = GetRequiredProfile(analyzerId);

        var mapping =
            profile.FieldMappings
                .FirstOrDefault(
                    x => x.Id == mappingId);

        if (mapping == null)
        {
            return false;
        }

        profile.FieldMappings.Remove(mapping);

        return true;
    }

    public bool RemoveResultTransformation(
        Guid analyzerId,
        Guid transformationId)
    {
        var profile = GetRequiredProfile(analyzerId);

        var transformation =
            profile.ResultTransformations
                .FirstOrDefault(
                    x => x.Id == transformationId);

        if (transformation == null)
        {
            return false;
        }

        profile.ResultTransformations.Remove(
            transformation);

        return true;
    }

    private AnalyzerMappingProfile
        GetRequiredProfile(Guid analyzerId)
    {
        var profile = GetProfile(analyzerId);

        if (profile == null)
        {
            throw new InvalidOperationException(
                $"No mapping profile exists for analyzer '{analyzerId}'.");
        }

        return profile;
    }

    public void AddUnitConversionRule(
    Guid analyzerId,
    UnitConversionRule rule)
    {
        var profile = GetRequiredProfile(analyzerId);

        rule.AnalyzerId = analyzerId;

        profile.UnitConversionRules.Add(rule);
    }

    public bool RemoveUnitConversionRule(
        Guid analyzerId,
        Guid ruleId)
    {
        var profile = GetRequiredProfile(analyzerId);

        var rule =
            profile.UnitConversionRules
                .FirstOrDefault(
                    x => x.Id == ruleId);

        if (rule == null)
        {
            return false;
        }

        profile.UnitConversionRules.Remove(rule);

        return true;
    }

    public AnalyzerMappingProfile EnsureProfile(
    Guid analyzerId,
    string analyzerName)
    {
        if (analyzerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Analyzer ID cannot be empty.",
                nameof(analyzerId));
        }

        if (string.IsNullOrWhiteSpace(analyzerName))
        {
            throw new ArgumentException(
                "Analyzer name is required.",
                nameof(analyzerName));
        }

        if (_profiles.TryGetValue(
                analyzerId,
                out AnalyzerMappingProfile? existingProfile))
        {
            return existingProfile;
        }

        return CreateProfile(
            analyzerId,
            analyzerName);
    }
}