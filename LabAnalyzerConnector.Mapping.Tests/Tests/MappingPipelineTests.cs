using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Models;
using LabAnalyzerConnector.Mapping.Services;
using Xunit;

namespace LabAnalyzerConnector.Mapping.Tests.Tests;

public class MappingPipelineTests
{
    [Fact]
    public void DifferentAnalyzers_CanMapDifferentTestCodes_ToSameStandardCode()
    {
        // Arrange

        var profileService =
            new AnalyzerMappingProfileService();

        var validationService =
            new MappingValidationService();

        // Create two different analyzers
        var nihonKohdenId =
            Guid.NewGuid();

        var sysmexId =
            Guid.NewGuid();

        // Create Nihon Kohden profile
        profileService.CreateProfile(
            nihonKohdenId,
            "Nihon Kohden MEK-9100");

        // Create Sysmex profile
        profileService.CreateProfile(
            sysmexId,
            "Sysmex XN-Series");

        // Nihon Kohden uses WBC_CNT
        profileService.AddTestCodeMapping(
            nihonKohdenId,
            new TestCodeMapping
            {
                Id = Guid.NewGuid(),
                AnalyzerId = nihonKohdenId,
                AnalyzerTestCode = "WBC_CNT",
                StandardTestCode = "WBC",
                IsActive = true
            });

        // Sysmex uses WBC
        profileService.AddTestCodeMapping(
            sysmexId,
            new TestCodeMapping
            {
                Id = Guid.NewGuid(),
                AnalyzerId = sysmexId,
                AnalyzerTestCode = "WBC",
                StandardTestCode = "WBC",
                IsActive = true
            });

        // Act

        var nihonKohdenProfile =
            profileService.GetProfile(
                nihonKohdenId);

        var sysmexProfile =
            profileService.GetProfile(
                sysmexId);

        // Validate profiles
        var nihonKohdenValidation =
            validationService.Validate(
                nihonKohdenProfile!);

        var sysmexValidation =
            validationService.Validate(
                sysmexProfile!);

        // Assert

        Assert.True(
            nihonKohdenValidation.IsValid);

        Assert.True(
            sysmexValidation.IsValid);

        Assert.Equal(
            "WBC",
            nihonKohdenProfile!
                .TestCodeMappings
                .Single()
                .StandardTestCode);

        Assert.Equal(
            "WBC",
            sysmexProfile!
                .TestCodeMappings
                .Single()
                .StandardTestCode);
    }
}