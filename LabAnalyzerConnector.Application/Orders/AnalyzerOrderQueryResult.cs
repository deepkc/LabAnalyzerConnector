namespace LabAnalyzerConnector.Application.Orders;

public sealed class AnalyzerOrderQueryResult
{
    public string OrderId { get; init; } =
        string.Empty;

    public string PatientId { get; init; } =
        string.Empty;

    public string PatientName { get; init; } =
        string.Empty;

    public string SpecimenId { get; init; } =
        string.Empty;

    public string Barcode { get; init; } =
        string.Empty;

    public IReadOnlyCollection<string>
        OrderedTests
    { get; init; } =
        Array.Empty<string>();

    public string Priority { get; init; } =
        "Routine";

    public string Status { get; init; } =
        "Pending";
}