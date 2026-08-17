using System.Collections.ObjectModel;
using System.Windows;
using LabAnalyzerConnector.Application.Orders;
using LabAnalyzerConnector.Application.Models;
using LabAnalyzerConnector.Application.Services;
using LabAnalyzerConnector.Communication.Managers;
using LabAnalyzerConnector.Communication.Managers.Events;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Enums;
using LabAnalyzerConnector.Core.Services;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;



namespace LabAnalyzerConnector;

public partial class MainWindow : Window
{
    private readonly AnalyzerManager _analyzerManager;

    private readonly ConnectionManager _connectionManager;

    private readonly AnalyzerConnectionCoordinator
        _connectionCoordinator;

  


    // =========================================================
    // ANALYZERS
    // =========================================================

    public ObservableCollection<AnalyzerStatusViewModel>
        Analyzers
    {
        get;
    } = new();


    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public MainWindow(
       AnalyzerManager analyzerManager,
       ConnectionManager connectionManager,
       AnalyzerConnectionCoordinator connectionCoordinator)
    {
        InitializeComponent();

        _analyzerManager =
            analyzerManager;

        _connectionManager =
            connectionManager;

        _connectionCoordinator =
            connectionCoordinator;

       


        // =====================================================
        // SET DATACONTEXT
        // =====================================================

        DataContext =
            this;


        // =====================================================
        // SUBSCRIBE TO EVENTS
        // =====================================================

        // Connection status changes
        _connectionManager.ConnectionStatusChanged +=
            OnConnectionStatusChanged;


        // Raw data received from analyzer
        _connectionManager.DataReceived +=
            OnDataReceived;


        // Analyzer configuration changes
        _analyzerManager.AnalyzerCollectionChanged +=
            OnAnalyzerCollectionChanged;


        // =====================================================
        // LOAD EXISTING ANALYZERS
        // =====================================================

        LoadAnalyzers();
    }


    // =========================================================
    // LOAD ANALYZERS
    // =========================================================

    private void LoadAnalyzers()
    {
        Analyzers.Clear();

        IReadOnlyCollection<AnalyzerConfiguration>
            configurations =
                _analyzerManager.GetAnalyzers();


        foreach (
            AnalyzerConfiguration configuration
            in configurations)
        {
            var viewModel =
                new AnalyzerStatusViewModel(
                    configuration.AnalyzerId,
                    configuration.Name,
                    configuration.Manufacturer,
                    configuration.Model);


            // Check whether a connection already exists

            if (_connectionManager.TryGetConnection(
                    configuration.AnalyzerId,
                    out var connection))
            {
                viewModel.Status =
                    connection.Status;
            }


            Analyzers.Add(
                viewModel);
        }
    }
   


    private static T? FindParent<T>(
    DependencyObject child)
    where T : DependencyObject
    {
        DependencyObject? parent =
            VisualTreeHelper.GetParent(
                child);

        while (parent is not null)
        {
            if (parent is T typedParent)
            {
                return typedParent;
            }

            parent =
                VisualTreeHelper.GetParent(
                    parent);
        }

        return null;
    }

    private static T? FindVisualChild<T>(
    DependencyObject parent)
    where T : DependencyObject
    {
        int childCount =
            VisualTreeHelper.GetChildrenCount(
                parent);

        for (int i = 0;
             i < childCount;
             i++)
        {
            DependencyObject child =
                VisualTreeHelper.GetChild(
                    parent,
                    i);

            if (child is T typedChild)
            {
                return typedChild;
            }

            T? result =
                FindVisualChild<T>(
                    child);

            if (result is not null)
            {
                return result;
            }
        }

        return null;
    }
    // =========================================================
    // CONNECTION STATUS CHANGED
    // =========================================================

    private void OnConnectionStatusChanged(
        object? sender,
        ConnectionStatusChangedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            AnalyzerStatusViewModel? analyzer =
                Analyzers.FirstOrDefault(
                    x =>
                        x.AnalyzerId ==
                        e.AnalyzerId);


            if (analyzer is null)
            {
                return;
            }


            analyzer.Status =
                e.Status;
        });
    }


    // =========================================================
    // RAW DATA RECEIVED
    // =========================================================

    private void OnDataReceived(
        object? sender,
        AnalyzerDataReceivedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            AnalyzerStatusViewModel? analyzer =
                Analyzers.FirstOrDefault(
                    x =>
                        x.AnalyzerId ==
                        e.AnalyzerId);


            if (analyzer is null)
            {
                return;
            }


            // Store the raw data received
            // from Compal / analyzer

            analyzer.UpdateReceivedData(
                e.Data);
        });
    }


    // =========================================================
    // ANALYZER COLLECTION CHANGED
    // =========================================================

    private void OnAnalyzerCollectionChanged(
        object? sender,
        EventArgs e)
    {
        Dispatcher.Invoke(
            LoadAnalyzers);
    }


    // =========================================================
    // CONNECT ANALYZER
    // =========================================================

    private async void ConnectAnalyzerButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }


        if (element.DataContext
            is not AnalyzerStatusViewModel analyzer)
        {
            return;
        }


        try
        {
            await _connectionCoordinator.ConnectAsync(
                analyzer.AnalyzerId);


            MessageBox.Show(
                $"Analyzer '{analyzer.Name}' connected successfully.",
                "Connected",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to connect to analyzer.\n\n{ex.Message}",
                "Connection Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    // =========================================================
    // DISCONNECT ANALYZER
    // =========================================================

    private async void DisconnectAnalyzerButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }


        if (element.DataContext
            is not AnalyzerStatusViewModel analyzer)
        {
            return;
        }


        try
        {
            await _connectionCoordinator.DisconnectAsync(
                analyzer.AnalyzerId);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to disconnect analyzer.\n\n{ex.Message}",
                "Disconnect Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    // =========================================================
    // RECONNECT ANALYZER
    // =========================================================

    private async void ReconnectAnalyzerButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }


        if (element.DataContext
            is not AnalyzerStatusViewModel analyzer)
        {
            return;
        }


        try
        {
            await _connectionCoordinator.ReconnectAsync(
                analyzer.AnalyzerId);


            MessageBox.Show(
                $"Analyzer '{analyzer.Name}' reconnected successfully.",
                "Reconnected",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to reconnect to analyzer.\n\n{ex.Message}",
                "Reconnect Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    // =========================================================
    // ADD ANALYZER
    // =========================================================

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


    // =========================================================
    // CLEANUP
    // =========================================================

    protected override void OnClosed(
        EventArgs e)
    {
        // Unsubscribe from connection status events

        _connectionManager.ConnectionStatusChanged -=
            OnConnectionStatusChanged;


        // Unsubscribe from incoming data events

        _connectionManager.DataReceived -=
            OnDataReceived;


        // Unsubscribe from analyzer collection events

        _analyzerManager.AnalyzerCollectionChanged -=
            OnAnalyzerCollectionChanged;


        base.OnClosed(e);
    }

    // =========================================================
    // CLEAR RECEIVED DATA
    // =========================================================

    private void ClearReceivedDataButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }

        if (element.DataContext
            is not AnalyzerStatusViewModel analyzer)
        {
            return;
        }

        analyzer.ClearReceivedMessages();
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


   
    private async void SendTestMessageButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }

        if (element.DataContext
            is not AnalyzerStatusViewModel analyzer)
        {
            return;
        }

        try
        {
            const string testMessage =
                "TEST ORDER FROM LAB ANALYZER CONNECTOR\r\n";

            await _connectionCoordinator.SendAsync(
                analyzer.AnalyzerId,
                testMessage);

            MessageBox.Show(
                "Test message sent successfully.",
                "Message Sent",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to send test message.\n\n{ex.Message}",
                "Send Error",
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

}