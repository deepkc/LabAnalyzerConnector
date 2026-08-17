using System.Globalization;
using LabAnalyzerConnector.Domain.Entities;
using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Services;

public sealed class MappingPipeline : IMappingPipeline
{
    private readonly IAnalyzerMappingProfileService _profileService;
    private readonly IFieldMappingEngine _fieldMappingEngine;
    private readonly ITestCodeMapper _testCodeMapper;
    private readonly IResultTransformationEngine _transformationEngine;
    private readonly IUnitConversionEngine _unitConversionEngine;
    private readonly IMappingValidationService _validationService;

    public MappingPipeline(
        IAnalyzerMappingProfileService profileService,
        IFieldMappingEngine fieldMappingEngine,
        ITestCodeMapper testCodeMapper,
        IResultTransformationEngine transformationEngine,
        IUnitConversionEngine unitConversionEngine,
        IMappingValidationService validationService)
    {
        _profileService = profileService;
        _fieldMappingEngine = fieldMappingEngine;
        _testCodeMapper = testCodeMapper;
        _transformationEngine = transformationEngine;
        _unitConversionEngine = unitConversionEngine;
        _validationService = validationService;
    }

    public async Task<MappingPipelineResult> ProcessAsync(
     Guid analyzerId,
     IReadOnlyDictionary<string, string?> sourceFields,
     CancellationToken cancellationToken = default)
    {
        var pipelineResult =
            new MappingPipelineResult();

        var profile =
            _profileService.GetProfile(analyzerId);

        if (profile == null)
        {
            pipelineResult.Errors.Add(
                $"No mapping profile found for analyzer '{analyzerId}'.");

            return pipelineResult;
        }

        var validationResult =
            _validationService.Validate(profile);

        if (!validationResult.IsValid)
        {
            pipelineResult.Errors.AddRange(
                validationResult.Errors);

            pipelineResult.Warnings.AddRange(
                validationResult.Warnings);

            return pipelineResult;
        }

        var mappedFields =
            _fieldMappingEngine.MapFields(
                analyzerId,
                profile.FieldMappings,
                sourceFields);


        System.Diagnostics.Debug.WriteLine(
    "========== FIELD MAPPING DEBUG ==========");

        System.Diagnostics.Debug.WriteLine(
            $"AnalyzerId = {analyzerId}");

        System.Diagnostics.Debug.WriteLine(
            "SOURCE FIELDS:");

        foreach (var sourceField in sourceFields)
        {
            System.Diagnostics.Debug.WriteLine(
                $"SOURCE [{sourceField.Key}] = [{sourceField.Value}]");
        }

        System.Diagnostics.Debug.WriteLine(
            "PROFILE FIELD MAPPINGS:");

        foreach (var mapping in profile.FieldMappings)
        {
            System.Diagnostics.Debug.WriteLine(
                $"MAPPING Source=[{mapping.SourceField}] " +
                $"Target=[{mapping.TargetField}] " +
                $"Required=[{mapping.IsRequired}] " +
                $"Active=[{mapping.IsActive}]");
        }

        System.Diagnostics.Debug.WriteLine(
            "MAPPED FIELDS:");

        foreach (var mappedField in mappedFields.Fields)
        {
            System.Diagnostics.Debug.WriteLine(
                $"MAPPED [{mappedField.Key}] = [{mappedField.Value}]");
        }

        System.Diagnostics.Debug.WriteLine(
            "==========================================");

        if (!mappedFields.IsValid)
        {
            pipelineResult.Errors.AddRange(
                mappedFields.MissingRequiredFields
                    .Select(
                        field =>
                            $"Required field '{field}' is missing."));

            return pipelineResult;
        }

        mappedFields.Fields.TryGetValue(
            "TestCode",
            out string? analyzerTestCode);

        mappedFields.Fields.TryGetValue(
            "ResultValue",
            out string? resultValue);

        mappedFields.Fields.TryGetValue(
            "Units",
            out string? units);

        mappedFields.Fields.TryGetValue(
            "ReferenceRange",
            out string? referenceRange);

        mappedFields.Fields.TryGetValue(
            "AbnormalFlag",
            out string? abnormalFlag);

        mappedFields.Fields.TryGetValue(
            "TestName",
            out string? testName);

        if (string.IsNullOrWhiteSpace(
         analyzerTestCode))
        {
            System.Diagnostics.Debug.WriteLine(
                "========== TEST CODE EXTRACTION FAILED ==========");

            System.Diagnostics.Debug.WriteLine(
                $"AnalyzerId = {analyzerId}");

            System.Diagnostics.Debug.WriteLine(
                $"Mapped TestCode = [{analyzerTestCode}]");

            System.Diagnostics.Debug.WriteLine(
                "Available mapped fields:");

            foreach (var field in mappedFields.Fields)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[{field.Key}] = [{field.Value}]");
            }

            System.Diagnostics.Debug.WriteLine(
                "==================================================");

            pipelineResult.Errors.Add(
                "Test code could not be extracted.");

            return pipelineResult;
        }

        System.Diagnostics.Debug.WriteLine(
    "========== TEST CODE MAPPING ==========");

        System.Diagnostics.Debug.WriteLine(
            $"AnalyzerId       = {analyzerId}");

        System.Diagnostics.Debug.WriteLine(
            $"AnalyzerTestCode = {analyzerTestCode}");

        System.Diagnostics.Debug.WriteLine(
            "=======================================");

        string? standardTestCode =
     await _testCodeMapper.MapToStandardCodeAsync(
         analyzerId,
         analyzerTestCode,
         cancellationToken);


        System.Diagnostics.Debug.WriteLine(
            "========== TEST CODE MAPPING ==========");

        System.Diagnostics.Debug.WriteLine(
            $"AnalyzerId       = {analyzerId}");

        System.Diagnostics.Debug.WriteLine(
            $"AnalyzerTestCode = {analyzerTestCode}");

       

        System.Diagnostics.Debug.WriteLine(
            $"StandardTestCode = {standardTestCode}");

        System.Diagnostics.Debug.WriteLine(
            "=======================================");

        if (string.IsNullOrWhiteSpace(
                standardTestCode))
        {
            pipelineResult.Errors.Add(
                $"No test code mapping found for '{analyzerTestCode}'.");

            return pipelineResult;
        }

        resultValue =
            _transformationEngine.Transform(
                analyzerId,
                standardTestCode,
                resultValue,
                profile.ResultTransformations);

        if (decimal.TryParse(
                resultValue,
                out decimal numericValue) &&
            !string.IsNullOrWhiteSpace(units))
        {
            decimal? convertedValue =
                _unitConversionEngine.Convert(
                    analyzerId,
                    standardTestCode,
                    numericValue,
                    units,
                    profile.UnitConversionRules);

            if (convertedValue.HasValue)
            {
                resultValue =
                    convertedValue.Value.ToString(
                        CultureInfo.InvariantCulture);
            }
      
        }




        var testCodeMapping =
     await _testCodeMapper.FindMappingAsync(
         analyzerId,
         analyzerTestCode,
         cancellationToken);

        if (testCodeMapping is null)
        {
            pipelineResult.Errors.Add(
                $"No test code mapping found for '{analyzerTestCode}'.");

            return pipelineResult;
        }

        var labResult =
     new LabResult
     {
         Id = Guid.NewGuid(),

         TestCode =
             testCodeMapping.StandardTestName,

         StandardTestCode =
             testCodeMapping.StandardTestCode,

         TestName =
             testCodeMapping.StandardTestName,

         ResultValue =
             resultValue,

         Units =
             units,

         ReferenceRange =
             referenceRange,

         AbnormalFlag =
             abnormalFlag,

         ResultDateTime =
             DateTime.UtcNow
     };

        pipelineResult.Result =
            labResult;

        pipelineResult.IsSuccess =
            true;

        return pipelineResult;
    }
}