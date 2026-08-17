using System.Windows;
using System.Windows.Controls;
using LabAnalyzerConnector.Application.Models;

namespace LabAnalyzerConnector.Controls;

public partial class AnalyzerCard : UserControl
{
    public AnalyzerCard()
    {
        InitializeComponent();
    }

    // =========================================================
    // ANALYZER
    // =========================================================

    public AnalyzerStatusViewModel Analyzer
    {
        get => (AnalyzerStatusViewModel)GetValue(AnalyzerProperty);
        set => SetValue(AnalyzerProperty, value);
    }

    public static readonly DependencyProperty AnalyzerProperty =
        DependencyProperty.Register(
            nameof(Analyzer),
            typeof(AnalyzerStatusViewModel),
            typeof(AnalyzerCard),
            new PropertyMetadata(null));


    // =========================================================
    // EVENTS
    // =========================================================

    public event EventHandler? ConnectRequested;

    public event EventHandler? DisconnectRequested;

    public event EventHandler? ReconnectRequested;

    public event EventHandler? ManageRequested;

    public event EventHandler? MessagesRequested;

    public event EventHandler<Guid>? ResultsRequested;


    // =========================================================
    // CONNECT
    // =========================================================

    private void ConnectButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ConnectRequested?.Invoke(
            this,
            EventArgs.Empty);
    }


    // =========================================================
    // DISCONNECT
    // =========================================================

    private void DisconnectButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DisconnectRequested?.Invoke(
            this,
            EventArgs.Empty);
    }


    // =========================================================
    // RECONNECT
    // =========================================================

    private void ReconnectButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ReconnectRequested?.Invoke(
            this,
            EventArgs.Empty);
    }


    // =========================================================
    // MANAGE
    // =========================================================

    private void ManageButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        ManageRequested?.Invoke(
            this,
            EventArgs.Empty);
    }


    // =========================================================
    // MESSAGES
    // =========================================================

    private void MessagesButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MessagesRequested?.Invoke(
            this,
            EventArgs.Empty);
    }


    // =========================================================
    // RESULTS
    // =========================================================

    private void ResultsButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (Analyzer is null)
        {
            return;
        }

        // Tell the parent/dashboard which analyzer
        // requested the Results screen.
        ResultsRequested?.Invoke(
            this,
            Analyzer.AnalyzerId);
    }
}