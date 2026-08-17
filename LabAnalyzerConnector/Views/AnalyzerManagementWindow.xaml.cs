using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using LabAnalyzerConnector.Application.Services;
using LabAnalyzerConnector.Application.ViewModels;
using LabAnalyzerConnector;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Mapping.Abstractions;


namespace LabAnalyzerConnector.Views;

public partial class AnalyzerManagementWindow : Window
{
    private readonly AnalyzerManagementViewModel _viewModel;
    private readonly AnalyzerManagementService _service;
    private readonly AnalyzerConnectionCoordinator _connectionCoordinator;

    public AnalyzerManagementWindow(
    AnalyzerManagementViewModel viewModel,
    AnalyzerManagementService service,
    AnalyzerConnectionCoordinator connectionCoordinator)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _service = service;
        _connectionCoordinator = connectionCoordinator;

        DataContext = _viewModel;
    }

    // =========================================================
    // ADD
    // =========================================================

    private async void AddButton_Click(
     object sender,
     RoutedEventArgs e)
    {
        try
        {
            var window =
                ActivatorUtilities.CreateInstance<AddAnalyzerWindow>(
                    ((App)System.Windows.Application.Current).Services);

            window.Owner = this;

            bool? result =
                window.ShowDialog();

            if (result != true)
            {
                return;
            }

            AnalyzerConfiguration? configuration =
                window.CreatedConfiguration;

            if (configuration is null)
            {
                return;
            }

            await _service.AddAnalyzerAsync(configuration);

            _viewModel.Refresh();

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
                "Add Analyzer Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void ConnectButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (_viewModel.SelectedAnalyzer == null)
        {
            MessageBox.Show(
                "Please select an analyzer.",
                "Connect Analyzer",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            await _connectionCoordinator.ConnectAsync(
                _viewModel.SelectedAnalyzer.AnalyzerId);

            MessageBox.Show(
                "Analyzer connection started.",
                "Connect Analyzer",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Connection Failed",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void DisconnectButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (_viewModel.SelectedAnalyzer == null)
        {
            MessageBox.Show(
                "Please select an analyzer.",
                "Disconnect Analyzer",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            await _connectionCoordinator.DisconnectAsync(
                _viewModel.SelectedAnalyzer.AnalyzerId);

            MessageBox.Show(
                "Analyzer disconnected.",
                "Disconnect Analyzer",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Disconnect Failed",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void ReconnectButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        if (_viewModel.SelectedAnalyzer == null)
        {
            MessageBox.Show(
                "Please select an analyzer.",
                "Reconnect Analyzer",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            await _connectionCoordinator.ReconnectAsync(
                _viewModel.SelectedAnalyzer.AnalyzerId);

            MessageBox.Show(
                "Analyzer reconnected successfully.",
                "Reconnect Analyzer",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to reconnect analyzer.\n\n{ex.Message}",
                "Reconnect Failed",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void ConnectAllButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            await _connectionCoordinator.ConnectAllAsync();

            _viewModel.Refresh();

            MessageBox.Show(
                "Connect All operation completed.",
                "Connect All",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Connect All failed.\n\n{ex.Message}",
                "Connect All",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void DisconnectAllButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            await _connectionCoordinator.DisconnectAllAsync();

            _viewModel.Refresh();

            MessageBox.Show(
                "All analyzers have been disconnected.",
                "Disconnect All",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Disconnect All failed.\n\n{ex.Message}",
                "Disconnect All",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // =========================================================
    // EDIT
    // =========================================================

    private async void EditButton_Click(
     object sender,
     RoutedEventArgs e)
    {
        if (_viewModel.SelectedAnalyzer == null)
        {
            MessageBox.Show(
                "Please select an analyzer.",
                "Edit Analyzer",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            AnalyzerConfiguration? configuration =
                _service.GetAnalyzer(
                    _viewModel.SelectedAnalyzer.AnalyzerId);

            if (configuration is null)
            {
                MessageBox.Show(
                    "The selected analyzer configuration could not be found.",
                    "Edit Analyzer",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var window =
                ActivatorUtilities.CreateInstance<AddAnalyzerWindow>(
                    ((App)System.Windows.Application.Current).Services,
                    configuration);

            window.Owner = this;

            bool? result =
                window.ShowDialog();

            if (result != true)
            {
                return;
            }

            AnalyzerConfiguration? updatedConfiguration =
                window.CreatedConfiguration;

            if (updatedConfiguration is null)
            {
                return;
            }


        

            if (_service.GetAnalyzer(
        updatedConfiguration.AnalyzerId) is null)
            {
               

                return;
            }

            await _service.UpdateAnalyzerAsync(
                updatedConfiguration);

            _viewModel.Refresh();

            MessageBox.Show(
                "Analyzer updated successfully.",
                "Edit Analyzer",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to edit analyzer.\n\n{ex.Message}",
                "Edit Analyzer Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // =========================================================
    // DELETE
    // =========================================================

    private async void DeleteButton_Click(
       object sender,
       RoutedEventArgs e)
    {
        if (_viewModel.SelectedAnalyzer == null)
        {
            MessageBox.Show(
                "Please select an analyzer.",
                "Delete Analyzer",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        MessageBoxResult result =
            MessageBox.Show(
                $"Delete analyzer '{_viewModel.SelectedAnalyzer.Name}' ?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            await _service.DeleteAnalyzerAsync(
                _viewModel.SelectedAnalyzer.AnalyzerId);

            _viewModel.Refresh();

            MessageBox.Show(
                "Analyzer deleted successfully.",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Delete Failed",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // =========================================================
    // DUPLICATE
    // =========================================================

    private async void DuplicateButton_Click(
     object sender,
     RoutedEventArgs e)
    {
        if (_viewModel.SelectedAnalyzer == null)
        {
            MessageBox.Show(
                "Please select an analyzer.",
                "Duplicate Analyzer",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            AnalyzerConfiguration? sourceConfiguration =
                _service.GetAnalyzer(
                    _viewModel.SelectedAnalyzer.AnalyzerId);

            if (sourceConfiguration is null)
            {
                MessageBox.Show(
                    "The selected analyzer configuration could not be found.",
                    "Duplicate Analyzer",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var duplicate =
                new AnalyzerConfiguration
                {
                    Id = Guid.NewGuid(),

                    AnalyzerId = Guid.NewGuid(),

                    Name =
                        $"{sourceConfiguration.Name} Copy",

                    Manufacturer =
                        sourceConfiguration.Manufacturer,

                    Model =
                        sourceConfiguration.Model,

                    SerialNumber =
                        sourceConfiguration.SerialNumber,

                    IsEnabled =
                        sourceConfiguration.IsEnabled,

                    AutoConnect =
                        false,

                    AutoReconnect =
                        sourceConfiguration.AutoReconnect,

                    ConnectionType =
                        sourceConfiguration.ConnectionType,

                    Direction =
                        sourceConfiguration.Direction,

                    Protocol =
                        sourceConfiguration.Protocol,

                    Tcp =
                        sourceConfiguration.Tcp,

                    Serial =
                        sourceConfiguration.Serial,

                    CreatedAtUtc =
                        DateTime.UtcNow,

                    UpdatedAtUtc =
                        DateTime.UtcNow
                };

            await _service.AddAnalyzerAsync(
                duplicate);

            _viewModel.Refresh();

            MessageBox.Show(
                "Analyzer duplicated successfully.",
                "Duplicate Analyzer",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to duplicate analyzer.\n\n{ex.Message}",
                "Duplicate Analyzer Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // =========================================================
    // REFRESH
    // =========================================================

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Refresh();
    }

    // =========================================================
    // MAPPINGS
    // =========================================================

    private void MappingButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_viewModel.SelectedAnalyzer == null)
        {
            MessageBox.Show(
                "Please select an analyzer.",
                "Analyzer Mapping",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            var app =
                (App)System.Windows.Application.Current;


            var repository =
                app.Services.GetRequiredService<
                    ITestCodeMappingRepository>();


            var window =
                new AnalyzerMappingWindow(
                    _viewModel.SelectedAnalyzer.AnalyzerId,
                    repository);


            window.Owner = this;

            window.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to open analyzer mappings.\n\n{ex.Message}",
                "Analyzer Mapping Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}