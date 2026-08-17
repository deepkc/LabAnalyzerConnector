using LabAnalyzerConnector.Application.Results;
using LabAnalyzerConnector.Domain.Entities;
using LabAnalyzerConnector.Infrastructure.Persistence;
using LabAnalyzerConnector.Infrastructure.Persistence.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LabAnalyzerConnector.Tests;

public sealed class LabResultPersistenceIntegrationTests
{
    [Fact]
    public async Task SaveAsync_ShouldPersistLabResult_AndRetrieveIt()
    {
        // =====================================================
        // ARRANGE
        // =====================================================

        await using SqliteConnection connection =
            new("Data Source=:memory:");

        await connection.OpenAsync();

        var options =
            new DbContextOptionsBuilder<
                LabAnalyzerDbContext>()
                .UseSqlite(connection)
                .Options;


        // Create database schema
        await using (
            var dbContext =
                new LabAnalyzerDbContext(options))
        {
            await dbContext.Database
                .EnsureCreatedAsync();
        }


        // Create repository
        var factory =
     new TestDbContextFactory<LabAnalyzerDbContext>(
         options);

        var repository =
            new SqliteLabResultRepository(
                factory);


        // Create persistence service
        var persistenceService =
            new LabResultPersistenceService(
                repository);


        Guid analyzerId =
            Guid.NewGuid();


        var result =
            new LabResult
            {
                Id =
                    Guid.NewGuid(),

                AnalyzerId =
                    analyzerId,

                AnalyzerName =
                    "Test Analyzer",

                PatientId =
                    "PAT-001",

                SampleId =
                    "SAMPLE-001",

                TestCode =
                    "HGB",

                StandardTestCode =
                    "LOINC-HGB",

                TestName =
                    "Hemoglobin",

                ResultValue =
                    "14.2",

                Units =
                    "g/dL",

                ReferenceRange =
                    "12.0-17.5",

                AbnormalFlag =
                    "N",

                ResultDateTime =
                    DateTime.UtcNow,

                ReceivedAtUtc =
                    DateTime.UtcNow,

                RawMessage =
                    "R|1|^^^HGB|14.2|g/dL|12.0-17.5|N"
            };


        // =====================================================
        // ACT
        // =====================================================

        await persistenceService.SaveAsync(
            result);


        LabResult? savedResult =
            await persistenceService.GetByIdAsync(
                result.Id);


        // =====================================================
        // ASSERT
        // =====================================================

        Assert.NotNull(
            savedResult);

        Assert.Equal(
            result.Id,
            savedResult.Id);

        Assert.Equal(
            analyzerId,
            savedResult.AnalyzerId);

        Assert.Equal(
            "Test Analyzer",
            savedResult.AnalyzerName);

        Assert.Equal(
            "PAT-001",
            savedResult.PatientId);

        Assert.Equal(
            "SAMPLE-001",
            savedResult.SampleId);

        Assert.Equal(
            "HGB",
            savedResult.TestCode);

        Assert.Equal(
            "LOINC-HGB",
            savedResult.StandardTestCode);

        Assert.Equal(
            "Hemoglobin",
            savedResult.TestName);

        Assert.Equal(
            "14.2",
            savedResult.ResultValue);

        Assert.Equal(
            "g/dL",
            savedResult.Units);

        Assert.Equal(
            "12.0-17.5",
            savedResult.ReferenceRange);

        Assert.Equal(
            "N",
            savedResult.AbnormalFlag);

        Assert.Equal(
            result.RawMessage,
            savedResult.RawMessage);
    }
}