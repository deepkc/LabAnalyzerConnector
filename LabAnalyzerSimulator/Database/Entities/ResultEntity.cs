namespace LabAnalyzerSimulator.Database.Entities;

public sealed class ResultEntity
{
    public Guid Id { get; set; }

    public string Barcode { get; set; } = string.Empty;

    public string TestCode { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;

    public string Units { get; set; } = string.Empty;

    public string ReferenceRange { get; set; } = string.Empty;

    public string Flag { get; set; } = "N";
}