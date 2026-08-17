namespace LabAnalyzerConnector.Core.Models;

public sealed class LabOrder
{
    public Guid Id { get; init; } =
        Guid.NewGuid();

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

    public List<string> OrderedTests { get; set; } =
        new();

    public string Priority { get; set; } =
        "Routine";

    public DateTime CreatedAt { get; set; } =
        DateTime.UtcNow;

    public string Status { get; set; } =
        "Pending";
}