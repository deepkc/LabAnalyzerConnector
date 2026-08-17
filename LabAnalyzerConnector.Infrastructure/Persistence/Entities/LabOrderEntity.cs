namespace LabAnalyzerConnector.Infrastructure.Persistence;

public sealed class LabOrderEntity
{
    public Guid Id { get; set; }

    public Guid? AnalyzerId { get; set; }

    public string OrderId { get; set; } =
        string.Empty;

    public string PatientId { get; set; } =
        string.Empty;

    public string PatientName { get; set; } =
        string.Empty;

    public string SpecimenId { get; set; } =
        string.Empty;

    public string Barcode { get; set; } =
        string.Empty;

    public string OrderedTests { get; set; } =
        string.Empty;

    public string Priority { get; set; } =
        "Routine";

    public DateTime CreatedAt { get; set; } =
        DateTime.UtcNow;

    public string Status { get; set; } =
        "Pending";
}