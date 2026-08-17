using LabAnalyzerSimulator.Database.Entities;
using LabAnalyzerSimulator.Repositories;
using LabAnalyzerSimulator.Models;

namespace LabAnalyzerSimulator.Services;

public sealed class SimulatorDataService
{
    private readonly IPatientRepository _patientRepository;

    private readonly IOrderRepository _orderRepository;

    private readonly IResultRepository _resultRepository;

    public SimulatorDataService(
        IPatientRepository patientRepository,
        IOrderRepository orderRepository,
        IResultRepository resultRepository)
    {
        _patientRepository = patientRepository;
        _orderRepository = orderRepository;
        _resultRepository = resultRepository;
    }

    public async Task<SimulatorPatientData?> GetPatientAsync(
        string barcode)
    {
        PatientEntity? patient =
            await _patientRepository.GetByBarcodeAsync(
                barcode);

        if (patient is null)
            return null;

        IReadOnlyCollection<OrderEntity> orders =
            await _orderRepository.GetOrdersByBarcodeAsync(
                barcode);

        IReadOnlyCollection<ResultEntity> results =
            await _resultRepository.GetResultsByBarcodeAsync(
                barcode);

        return new SimulatorPatientData
        {
            Patient = patient,
            Orders = orders,
            Results = results
        };
    }
}