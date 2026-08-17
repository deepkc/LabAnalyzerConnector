namespace LabAnalyzerConnector.Domain.Entities;

public sealed class NormalizedLabMessage
{
    public Guid Id { get; set; }

    public Guid AnalyzerId { get; set; }

    public string? AnalyzerName { get; set; }

    public string? PatientId { get; set; }

    public string? SampleId { get; set; }

    public string? Barcode { get; set; }

    public string? AccessionNumber { get; set; }

    public DateTime ReceivedAtUtc { get; set; }

    public string? RawMessage { get; set; }

    public List<LabResult> Results { get; set; } = new();
}