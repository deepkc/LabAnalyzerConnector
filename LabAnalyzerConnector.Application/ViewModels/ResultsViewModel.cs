using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using LabAnalyzerConnector.Application.Results;
using LabAnalyzerConnector.Domain.Entities;

namespace LabAnalyzerConnector.Application.ViewModels;

public sealed class ResultsViewModel
    : INotifyPropertyChanged
{
    private readonly LabResultPersistenceService _service;

    public Guid AnalyzerId { get; }

    public ObservableCollection<LabResult> Results { get; }
        = new();

    public ResultsViewModel(
        Guid analyzerId,
        LabResultPersistenceService service)
    {
        AnalyzerId = analyzerId;

        _service =
            service
            ?? throw new ArgumentNullException(
                nameof(service));
    }

    public async Task LoadAsync(
        CancellationToken cancellationToken = default)
    {
        Results.Clear();

        IReadOnlyCollection<LabResult> results =
            await _service.GetByAnalyzerIdAsync(
                AnalyzerId,
                cancellationToken);

        foreach (LabResult result in results)
        {
            Results.Add(result);
        }
    }

    public event PropertyChangedEventHandler?
        PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName]
        string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                propertyName));
    }
}