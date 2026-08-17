using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Application.Models;

public sealed class AnalyzerStatusViewModel : INotifyPropertyChanged
{
    // =========================================================
    // PRIVATE FIELDS
    // =========================================================

    private ConnectionStatus _status =
        ConnectionStatus.Disconnected;

    private string _lastReceivedData =
        string.Empty;

    private DateTime? _lastReceivedAt;


    // =========================================================
    // ANALYZER INFORMATION
    // =========================================================

    public Guid AnalyzerId
    {
        get;
    }

    public string Name
    {
        get;
    }

    public string Manufacturer
    {
        get;
    }

    public string Model
    {
        get;
    }


    // =========================================================
    // CONNECTION STATUS
    // =========================================================

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
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(IsConnected));
            OnPropertyChanged(nameof(StatusColor));
        }
    }


    // =========================================================
    // STATUS TEXT
    // =========================================================

    public string StatusText =>
        Status.ToString();


    // =========================================================
    // CONNECTION CHECK
    // =========================================================

    public bool IsConnected =>
        Status == ConnectionStatus.Connected;


    // =========================================================
    // STATUS COLOR
    // =========================================================

    public string StatusColor
    {
        get
        {
            return Status switch
            {
                ConnectionStatus.Connected =>
                    "#16A34A",

                ConnectionStatus.Connecting =>
                    "#F59E0B",

                ConnectionStatus.Error =>
                    "#DC2626",

                _ =>
                    "#9CA3AF"
            };
        }
    }


    // =========================================================
    // LAST RECEIVED DATA
    // =========================================================

    public string LastReceivedData
    {
        get => _lastReceivedData;

        private set
        {
            if (_lastReceivedData == value)
            {
                return;
            }

            _lastReceivedData = value;

            OnPropertyChanged();
        }
    }


    // =========================================================
    // LAST RECEIVED TIME
    // =========================================================

    public DateTime? LastReceivedAt
    {
        get => _lastReceivedAt;

        private set
        {
            if (_lastReceivedAt == value)
            {
                return;
            }

            _lastReceivedAt = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(LastReceivedText));
        }
    }


    // =========================================================
    // LAST RECEIVED TIME TEXT
    // =========================================================

    public string LastReceivedText
    {
        get
        {
            if (LastReceivedAt is null)
            {
                return "No data received yet";
            }

            return LastReceivedAt.Value
                .ToLocalTime()
                .ToString("yyyy-MM-dd HH:mm:ss");
        }
    }


    // =========================================================
    // RECEIVED MESSAGE HISTORY
    // =========================================================

    public ObservableCollection<ReceivedMessageViewModel>
        ReceivedMessages
    {
        get;
    } = new();


    // =========================================================
    // PROPERTY CHANGED EVENT
    // =========================================================

    public event PropertyChangedEventHandler?
        PropertyChanged;


    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public AnalyzerStatusViewModel(
        Guid analyzerId,
        string name,
        string manufacturer,
        string model)
    {
        AnalyzerId =
            analyzerId;

        Name =
            name;

        Manufacturer =
            manufacturer;

        Model =
            model;
    }


    // =========================================================
    // UPDATE RECEIVED DATA
    // =========================================================

    public void UpdateReceivedData(
        string data)
    {
        DateTime receivedAt =
            DateTime.Now;


        // -----------------------------------------------------
        // 1. Update latest received message
        // -----------------------------------------------------

        LastReceivedData =
            data;

        LastReceivedAt =
            receivedAt;


        // -----------------------------------------------------
        // 2. Add message to complete history
        // -----------------------------------------------------

        ReceivedMessages.Add(
            new ReceivedMessageViewModel(
                data,
                receivedAt));
    }


    // =========================================================
    // CLEAR RECEIVED MESSAGE HISTORY
    // =========================================================

    public void ClearReceivedMessages()
    {
        ReceivedMessages.Clear();

        LastReceivedData =
            string.Empty;

        LastReceivedAt =
            null;
    }


    // =========================================================
    // RAISE PROPERTY CHANGED
    // =========================================================

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