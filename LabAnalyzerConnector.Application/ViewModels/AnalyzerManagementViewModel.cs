using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LabAnalyzerConnector.Communication.Managers;
using LabAnalyzerConnector.Communication.Managers.Events;
using LabAnalyzerConnector.Core.Enums;
using LabAnalyzerConnector.Application.Models;
using LabAnalyzerConnector.Application.Services;
using System.Windows;

namespace LabAnalyzerConnector.Application.ViewModels;

public sealed class AnalyzerManagementViewModel
    : INotifyPropertyChanged
{
    private readonly AnalyzerManagementService _service;
    private readonly ConnectionManager _connectionManager;
    public ObservableCollection<AnalyzerListItem> Analyzers
    {
        get;
    } = new();

    private AnalyzerListItem? _selectedAnalyzer;

    public AnalyzerListItem? SelectedAnalyzer
    {
        get => _selectedAnalyzer;
        set
        {
            _selectedAnalyzer = value;
            OnPropertyChanged();
        }
    }

    public AnalyzerManagementViewModel(
     AnalyzerManagementService service,
     ConnectionManager connectionManager)
    {
        _service = service;
        _connectionManager = connectionManager;

        _connectionManager.ConnectionStatusChanged +=
            ConnectionManager_ConnectionStatusChanged;

        Refresh();
    }

    public void Refresh()
    {
        Analyzers.Clear();

        foreach (var analyzer in _service.GetAnalyzers())
        {
            Analyzers.Add(analyzer);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }

    private void ConnectionManager_ConnectionStatusChanged(
    object? sender,
    ConnectionStatusChangedEventArgs e)
    {
        AnalyzerListItem? analyzer =
            Analyzers.FirstOrDefault(
                x => x.AnalyzerId == e.AnalyzerId);

        if (analyzer == null)
        {
            return;
        }

        analyzer.Status = e.Status;
    }
}