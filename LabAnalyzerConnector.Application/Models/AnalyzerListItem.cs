using LabAnalyzerConnector.Core.Enums;
using System.ComponentModel;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LabAnalyzerConnector.Application.Models;

public sealed class AnalyzerListItem
    : INotifyPropertyChanged
{
    public Guid AnalyzerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Manufacturer { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public ProtocolType Protocol { get; set; }

    public ConnectionType ConnectionType { get; set; }

    private ConnectionStatus _status;

    public ConnectionStatus Status
    {
        get => _status;
        set
        {
            if (_status == value)
            {
                return;
            }

            _status = value;
            OnPropertyChanged();
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

    public bool Enabled { get; set; }
}