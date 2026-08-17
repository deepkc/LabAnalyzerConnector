using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Services;

public sealed class AnalyzerMappingInitializer
{
    private readonly IAnalyzerMappingProfileService _profileService;

    public AnalyzerMappingInitializer(
        IAnalyzerMappingProfileService profileService)
    {
        _profileService = profileService;
    }

    public void Initialize(
        Guid analyzerId,
        string analyzerName)
    {
        if (_profileService.GetProfile(analyzerId) != null)
        {
            return;
        }

        AnalyzerMappingProfile profile =
            _profileService.CreateProfile(
                analyzerId,
                analyzerName);

        // =====================================================
        // FIELD MAPPINGS
        // =====================================================

        profile.FieldMappings.Add(
            new FieldMapping
            {
                Id = Guid.NewGuid(),
                AnalyzerId = analyzerId,
                MessageType = "HL7",
                SourceField = "TestCode",
                TargetField = "TestCode",
                IsRequired = true,
                IsActive = true
            });

        profile.FieldMappings.Add(
            new FieldMapping
            {
                Id = Guid.NewGuid(),
                AnalyzerId = analyzerId,
                MessageType = "HL7",
                SourceField = "TestName",
                TargetField = "TestName",
                IsRequired = false,
                IsActive = true
            });

        profile.FieldMappings.Add(
            new FieldMapping
            {
                Id = Guid.NewGuid(),
                AnalyzerId = analyzerId,
                MessageType = "HL7",
                SourceField = "ResultValue",
                TargetField = "ResultValue",
                IsRequired = false,
                IsActive = true
            });

        profile.FieldMappings.Add(
            new FieldMapping
            {
                Id = Guid.NewGuid(),
                AnalyzerId = analyzerId,
                MessageType = "HL7",
                SourceField = "Units",
                TargetField = "Units",
                IsRequired = false,
                IsActive = true
            });

        profile.FieldMappings.Add(
            new FieldMapping
            {
                Id = Guid.NewGuid(),
                AnalyzerId = analyzerId,
                MessageType = "HL7",
                SourceField = "ReferenceRange",
                TargetField = "ReferenceRange",
                IsRequired = false,
                IsActive = true
            });

        profile.FieldMappings.Add(
            new FieldMapping
            {
                Id = Guid.NewGuid(),
                AnalyzerId = analyzerId,
                MessageType = "HL7",
                SourceField = "AbnormalFlag",
                TargetField = "AbnormalFlag",
                IsRequired = false,
                IsActive = true
            });

        // =====================================================
        // TEST CODE MAPPINGS
        // =====================================================

        AddMapping(
            analyzerId,
            "WBC",
            "6690-2",
            "White Blood Cell Count");

        AddMapping(
            analyzerId,
            "NEU#",
            "751-8",
            "Neutrophils");

        AddMapping(
            analyzerId,
            "LYM#",
            "731-0",
            "Lymphocytes");

        AddMapping(
            analyzerId,
            "MON#",
            "742-7",
            "Monocytes");

        AddMapping(
            analyzerId,
            "EOS#",
            "711-2",
            "Eosinophils");

        AddMapping(
            analyzerId,
            "BAS#",
            "704-7",
            "Basophils");

        AddMapping(
            analyzerId,
            "RBC",
            "789-8",
            "Red Blood Cell Count");

        AddMapping(
            analyzerId,
            "HGB",
            "718-7",
            "Hemoglobin");

        AddMapping(
            analyzerId,
            "HCT",
            "4544-3",
            "Hematocrit");

        AddMapping(
            analyzerId,
            "MCV",
            "787-2",
            "Mean Corpuscular Volume");

        AddMapping(
            analyzerId,
            "MCH",
            "785-6",
            "Mean Corpuscular Hemoglobin");

        AddMapping(
            analyzerId,
            "MCHC",
            "786-4",
            "Mean Corpuscular Hemoglobin Concentration");

        AddMapping(
            analyzerId,
            "RDW-CV",
            "21000-5",
            "Red Cell Distribution Width");

        AddMapping(
            analyzerId,
            "PLT",
            "777-3",
            "Platelet Count");

        AddMapping(
            analyzerId,
            "MPV",
            "32623-1",
            "Mean Platelet Volume");
    }



    private void AddMapping(
        Guid analyzerId,
        string analyzerTestCode,
        string loincCode,
        string standardTestName)
    {
        _profileService.AddTestCodeMapping(
            analyzerId,
            new TestCodeMapping
            {
                Id = Guid.NewGuid(),

                AnalyzerId = analyzerId,

                AnalyzerTestCode =
                    analyzerTestCode,

                StandardTestCode =
                    loincCode,

                StandardTestName =
                    standardTestName,

                IsActive = true
            });
    }
}