namespace LabAnalyzerSimulator.Database.Entities;

public sealed class OrderEntity
{
    public Guid Id { get; set; }

    public string Barcode { get; set; } = string.Empty;

    public string TestCode { get; set; } = string.Empty;

    public string TestName { get; set; } = string.Empty;

    public string Priority { get; set; } = "Routine";
}