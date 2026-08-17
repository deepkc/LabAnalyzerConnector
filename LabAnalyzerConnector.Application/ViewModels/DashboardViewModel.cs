using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LabAnalyzerConnector.Application.Events;


namespace LabAnalyzerConnector.Application.ViewModels;

public sealed class DashboardViewModel : INotifyPropertyChanged
{
    public ObservableCollection<DashboardEvent> RecentEvents { get; }
        = new();

    private int _messagesReceived;

    public int MessagesReceived
    {
        get => _messagesReceived;
        set
        {
            _messagesReceived = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ActivityItemViewModel> Activity
    {
        get;
    }
=
new();

    private int _resultsSaved;

    public int ResultsSaved
    {
        get => _resultsSaved;
        set
        {
            _resultsSaved = value;
            OnPropertyChanged();
        }
    }

    private int _errors;

    public int Errors
    {
        get => _errors;
        set
        {
            _errors = value;
            OnPropertyChanged();
        }
    }

    public DashboardViewModel(
        DashboardEventBus eventBus)
    {
        eventBus.EventPublished += EventBus_EventPublished;
    }

    private void EventBus_EventPublished(
     object? sender,
     DashboardEventArgs e)
    {
        RecentEvents.Insert(0, e.Event);

        while (RecentEvents.Count > 100)
            RecentEvents.RemoveAt(100);

        Activity.Insert(0,
            new ActivityItemViewModel
            {
                Time = e.Event.Time,
                Analyzer = e.Event.AnalyzerName,
                Message = e.Event.Message,
                Type = e.Event.Type.ToString()
            });

        while (Activity.Count > 200)
            Activity.RemoveAt(200);

        switch (e.Event.Type)
        {
            case DashboardEventType.MessageReceived:
                MessagesReceived++;
                break;

            case DashboardEventType.ResultSaved:
                ResultsSaved++;
                break;

            case DashboardEventType.Error:
                Errors++;
                break;
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
}