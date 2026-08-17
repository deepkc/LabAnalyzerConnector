using LabAnalyzerSimulator.Database.Entities;

namespace LabAnalyzerSimulator.Models;

public sealed class SimulatorPatientData
{
    public PatientEntity Patient { get; set; } = null!;

    public IReadOnlyCollection<OrderEntity> Orders
    {
        get;
        set;
    } = Array.Empty<OrderEntity>();

    public IReadOnlyCollection<ResultEntity> Results
    {
        get;
        set;
    } = Array.Empty<ResultEntity>();
}