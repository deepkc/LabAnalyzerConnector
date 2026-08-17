using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Abstractions;

public interface IAnalyzerMappingProfileService
{
    AnalyzerMappingProfile CreateProfile(
        Guid analyzerId,
        string analyzerName);

    AnalyzerMappingProfile? GetProfile(
        Guid analyzerId);

    void AddTestCodeMapping(
        Guid analyzerId,
        TestCodeMapping mapping);

    void AddUnitConversionRule(
    Guid analyzerId,
    UnitConversionRule rule);

    bool RemoveUnitConversionRule(
        Guid analyzerId,
        Guid ruleId);

    void AddFieldMapping(
        Guid analyzerId,
        FieldMapping mapping);

    void AddResultTransformation(
        Guid analyzerId,
        ResultTransformation transformation);

    bool RemoveTestCodeMapping(
        Guid analyzerId,
        Guid mappingId);

    bool RemoveFieldMapping(
        Guid analyzerId,
        Guid mappingId);

    bool RemoveResultTransformation(
        Guid analyzerId,
        Guid transformationId);

    AnalyzerMappingProfile EnsureProfile(
    Guid analyzerId,
    string analyzerName);
}