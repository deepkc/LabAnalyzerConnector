namespace LabAnalyzerConnector.Domain.Entities;

public sealed class LabResult
{
    public Guid Id { get; set; }

    // =====================================================
    // Source Analyzer
    // =====================================================

    public Guid AnalyzerId { get; set; }

    public string? AnalyzerName { get; set; }

    // =====================================================
    // Patient / Sample Identity
    // =====================================================

    public string? PatientId { get; set; }

    public string? SampleId { get; set; }

    // =====================================================
    // Test Information
    // =====================================================

    public string? TestCode { get; set; }

    public string? StandardTestCode { get; set; }

    public string? TestName { get; set; }

    // =====================================================
    // Result
    // =====================================================

    public string? ResultValue { get; set; }

    public string? Units { get; set; }

    public string? ReferenceRange { get; set; }

    public string? AbnormalFlag { get; set; }

    // =====================================================
    // Timing
    // =====================================================

    public DateTime ResultDateTime { get; set; }

    public DateTime ReceivedAtUtc { get; set; }

    // =====================================================
    // Audit / Raw Data
    // =====================================================

    public string? RawMessage { get; set; }
}