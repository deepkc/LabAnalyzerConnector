namespace LabAnalyzerConnector.Infrastructure.Persistence.Entities;

public sealed class LabResultEntity
{
    public Guid Id { get; set; }

    // =====================================================
    // SOURCE ANALYZER
    // =====================================================

    public Guid AnalyzerId { get; set; }

    public string? AnalyzerName { get; set; }


    // =====================================================
    // PATIENT / SAMPLE
    // =====================================================

    public string? PatientId { get; set; }

    public string? SampleId { get; set; }


    // =====================================================
    // TEST INFORMATION
    // =====================================================

    public string? TestCode { get; set; }

    public string? StandardTestCode { get; set; }

    public string? TestName { get; set; }


    // =====================================================
    // RESULT
    // =====================================================

    public string? ResultValue { get; set; }

    public string? Units { get; set; }

    public string? ReferenceRange { get; set; }

    public string? AbnormalFlag { get; set; }


    // =====================================================
    // TIMING
    // =====================================================

    public DateTime ResultDateTime { get; set; }

    public DateTime ReceivedAtUtc { get; set; }


    // =====================================================
    // AUDIT / RAW DATA
    // =====================================================

    public string? RawMessage { get; set; }
}