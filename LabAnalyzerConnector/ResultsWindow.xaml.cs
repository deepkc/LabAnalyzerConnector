using System.Windows;
using LabAnalyzerConnector.Application.Results;
using LabAnalyzerConnector.Application.ViewModels;
using Microsoft.Extensions.DependencyInjection;


namespace LabAnalyzerConnector;

public partial class ResultsWindow : Window
{
    private readonly ResultsViewModel _viewModel;



    public ResultsWindow(
     ResultsViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        DataContext = _viewModel;

        Loaded += ResultsWindow_Loaded;
    }


    private async void ResultsWindow_Loaded(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            await _viewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to load results.\n\n{ex.Message}",
                "Results",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    private async void RefreshButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await _viewModel.LoadAsync();
    }


    private void CloseButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
}