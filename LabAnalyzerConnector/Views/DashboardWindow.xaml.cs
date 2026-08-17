using System.Collections.ObjectModel;
using System.Windows;
using LabAnalyzerConnector.Application.Models;
using LabAnalyzerConnector.Application.Services;
using LabAnalyzerConnector.Communication.Managers;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using LabAnalyzerConnector.Controls;
using LabAnalyzerConnector.Core.Enums;
using LabAnalyzerConnector.Communication.Managers.Events;
using LabAnalyzerConnector.Views;
using LabAnalyzerConnector.Application.ViewModels;
using LabAnalyzerConnector.Application.Results;

namespace LabAnalyzerConnector;

public partial class DashboardWindow : Window
{
    private readonly AnalyzerManager _analyzerManager;
    private readonly ConnectionManager _connectionManager;
    private readonly AnalyzerConnectionCoordinator _connectionCoordinator;
    private readonly DashboardViewModel _dashboardViewModel;


    public ObservableCollection<AnalyzerStatusViewModel> Analyzers
    {
        get;
    } = new();

    public DashboardWindow(
    AnalyzerManager analyzerManager,
    ConnectionManager connectionManager,
    AnalyzerConnectionCoordinator connectionCoordinator,
    DashboardViewModel dashboardViewModel)
    {
        InitializeComponent();

        _analyzerManager = analyzerManager;
        _connectionManager = connectionManager;
        _connectionCoordinator = connectionCoordinator;
        _dashboardViewModel = dashboardViewModel;
        _connectionManager.ConnectionStatusChanged += ConnectionManager_ConnectionStatusChanged;

        _connectionManager.DataReceived += ConnectionManager_DataReceived;

        _connectionManager.ErrorOccurred += ConnectionManager_ErrorOccurred;

        Dashboard = dashboardViewModel;

        DataContext = this;

        LoadAnalyzers();
    }

    private void LoadAnalyzers()
    {
        Analyzers.Clear();

        IReadOnlyCollection<AnalyzerConfiguration> analyzers =
            _analyzerManager.GetAnalyzers();

        foreach (AnalyzerConfiguration analyzer in analyzers)
        {
            AnalyzerStatusViewModel vm =
                new(
                    analyzer.AnalyzerId,
                    analyzer.Name,
                    analyzer.Manufacturer,
                    analyzer.Model);

            Analyzers.Add(vm);
        }
    }


    private void OrdersButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            OrdersWindow ordersWindow =
                ActivatorUtilities.CreateInstance<OrdersWindow>(
                    ((App)System.Windows.Application.Current).Services);

            ordersWindow.Owner = this;

            ordersWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to open Orders window.\n\n{ex.Message}",
                "Orders Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    private void CreateOrderButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            var createOrderWindow =
                ActivatorUtilities.CreateInstance<CreateOrderWindow>(
                    ((App)System.Windows.Application.Current).Services);

            createOrderWindow.Owner = this;

            createOrderWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to open Create Order window.\n\n{ex.Message}",
                "Create Order Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void AddAnalyzerButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var addAnalyzerWindow =
     ActivatorUtilities.CreateInstance<AddAnalyzerWindow>(
         ((App)System.Windows.Application.Current).Services);

        addAnalyzerWindow.Owner = this;


        bool? result =
            addAnalyzerWindow.ShowDialog();


        if (result != true)
        {
            return;
        }


        AnalyzerConfiguration? configuration =
            addAnalyzerWindow.CreatedConfiguration;


        if (configuration is null)
        {
            return;
        }


        try
        {
            await _analyzerManager.AddAnalyzerAsync(
                configuration);
            LoadAnalyzers();


            MessageBox.Show(
                $"Analyzer '{configuration.Name}' was added successfully.",
                "Analyzer Added",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to add analyzer.\n\n{ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void AnalyzerCard_Loaded(
    object sender,
    RoutedEventArgs e)
    {
        if (sender is not AnalyzerCard card)
            return;

        card.ConnectRequested -= Card_ConnectRequested;
        card.DisconnectRequested -= Card_DisconnectRequested;
        card.ReconnectRequested -= Card_ReconnectRequested;
        card.MessagesRequested -= Card_MessagesRequested;
        card.ResultsRequested -= Card_ResultsRequested;
        card.ManageRequested -= Card_ManageRequested;

        card.ConnectRequested += Card_ConnectRequested;
        card.DisconnectRequested += Card_DisconnectRequested;
        card.ReconnectRequested += Card_ReconnectRequested;
        card.MessagesRequested += Card_MessagesRequested;
        card.ResultsRequested += Card_ResultsRequested;
        card.ManageRequested += Card_ManageRequested;
    }

    private async void Card_ConnectRequested(object? sender, EventArgs e)
    {
        if (sender is not AnalyzerCard card)
            return;

        try
        {
            await _connectionCoordinator.ConnectAsync(card.Analyzer.AnalyzerId);

           
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message,
                "Connect Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void Card_DisconnectRequested(object? sender, EventArgs e)
    {
        if (sender is not AnalyzerCard card)
            return;

        try
        {
            await _connectionCoordinator.DisconnectAsync(card.Analyzer.AnalyzerId);

            
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message,
                "Disconnect Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void Card_ReconnectRequested(object? sender, EventArgs e)
    {
        if (sender is not AnalyzerCard card)
            return;

        try
        {
            await _connectionCoordinator.ReconnectAsync(card.Analyzer.AnalyzerId);

           
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message,
                "Reconnect Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void Card_ManageRequested(object? sender, EventArgs e)
    {
        if (sender is not AnalyzerCard card)
            return;

        MessageBox.Show(
            $"Manage {card.Analyzer.Name}");
    }

    private void Card_MessagesRequested(object? sender, EventArgs e)
    {
        if (sender is not AnalyzerCard card)
            return;

        MessageBox.Show(
            $"Messages {card.Analyzer.Name}");
    }

    private void Card_ResultsRequested(
      object? sender,
      Guid analyzerId)
    {
        OpenResultsWindow(analyzerId);
    }
    public DashboardViewModel Dashboard
    {
        get;
    }
    private void ConnectionManager_ConnectionStatusChanged(
    object? sender,
    ConnectionStatusChangedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            AnalyzerStatusViewModel? analyzer =
                Analyzers.FirstOrDefault(a => a.AnalyzerId == e.AnalyzerId);

            if (analyzer == null)
                return;

            analyzer.Status = e.Status;
        });
    }

    private void ConnectionManager_DataReceived(
    object? sender,
    AnalyzerDataReceivedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            AnalyzerStatusViewModel? analyzer =
                Analyzers.FirstOrDefault(a => a.AnalyzerId == e.AnalyzerId);

            if (analyzer == null)
                return;

            analyzer.UpdateReceivedData(e.Data);
        });
    }


    private void OpenResultsWindow(
    Guid analyzerId)
    {
        var services =
            ((App)System.Windows.Application.Current).Services;

        var persistenceService =
            Microsoft.Extensions.DependencyInjection
                .ServiceProviderServiceExtensions
                .GetRequiredService<LabResultPersistenceService>(
                    services);

        var viewModel =
            new ResultsViewModel(
                analyzerId,
                persistenceService);

        var window =
            new ResultsWindow(
                viewModel);

        window.Owner = this;

        window.ShowDialog();
    }
    private void ConnectionManager_ErrorOccurred(
    object? sender,
    AnalyzerErrorEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            AnalyzerStatusViewModel? analyzer =
                Analyzers.FirstOrDefault(a => a.AnalyzerId == e.AnalyzerId);

            if (analyzer == null)
                return;

            analyzer.Status = ConnectionStatus.Error;
        });
    }

    private void TestConsoleButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        var window =
           ActivatorUtilities.CreateInstance<TestConsoleWindow>(
    ((App)System.Windows.Application.Current).Services);

        window.Owner = this;

        window.Show();
    }

    private void AnalyzerManagementButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        var window =
     ActivatorUtilities.CreateInstance<AnalyzerManagementWindow>(
         ((App)System.Windows.Application.Current).Services);

        window.Owner = this;

        window.ShowDialog();
    }
}