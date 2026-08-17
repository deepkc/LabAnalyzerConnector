namespace LabAnalyzerSimulator.Database.Entities;

public sealed class PatientEntity
{
    public Guid Id { get; set; }

    public string Barcode { get; set; } = string.Empty;

    public string PatientId { get; set; } = string.Empty;

    public string PatientName { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}