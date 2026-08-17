using System;
using System.Collections.ObjectModel;
using System.Windows;
using LabAnalyzerSimulator.Communication;
using LabAnalyzerSimulator.Protocols.ASTM;
using LabAnalyzerSimulator.Protocols.ASTM.Builders;
using LabAnalyzerSimulator.Protocols.ASTM.Generators;

namespace LabAnalyzerSimulator;

public partial class MainWindow : Window
{
    private readonly TcpServerService _server;

    private readonly AstmOrderParser _orderParser = new();

    private readonly AstmResultGenerator _resultGenerator = new();

    private readonly AstmResultMessageBuilder _resultBuilder = new();

    private readonly ObservableCollection<AstmQueryOrder> _receivedOrders =
        new();

    public MainWindow()
    {
        InitializeComponent();

        OrdersGrid.ItemsSource = _receivedOrders;

        _server = new TcpServerService();

        _server.DataReceived += Server_DataReceived;
        _server.StatusChanged += Server_StatusChanged;
    }

    private async void StartButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            if (!int.TryParse(
                    PortTextBox.Text,
                    out int port))
            {
                MessageBox.Show("Invalid port.");
                return;
            }

            await _server.StartAsync(port);

            StatusTextBlock.Text = "Running";

            CommunicationLogTextBox.AppendText(
                $"Server started on port {port}\r\n");

            CommunicationLogTextBox.ScrollToEnd();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void StopButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        _server.Stop();

        StatusTextBlock.Text = "Stopped";

        CommunicationLogTextBox.AppendText(
            "Server stopped.\r\n");

        CommunicationLogTextBox.ScrollToEnd();
    }

 

    private async void Server_DataReceived(
        string message)
    {
        Dispatcher.Invoke(() =>
        {
            CommunicationLogTextBox.AppendText(
                Environment.NewLine);

            CommunicationLogTextBox.AppendText(
                "========== ASTM RECEIVED ==========\r\n");

            CommunicationLogTextBox.AppendText(
                message + Environment.NewLine);

            CommunicationLogTextBox.ScrollToEnd();
        });

        AstmQueryOrder? order =
            _orderParser.Parse(message);

        if (order == null)
            return;

        Dispatcher.Invoke(() =>
        {
            _receivedOrders.Add(order);

            CommunicationLogTextBox.AppendText(
                $"Parsed Barcode: {order.Barcode}\r\n");
        });

        // Generate fake analyzer results
        var results =
            _resultGenerator.GenerateResults(
                order.Barcode);

        // Build ASTM Result Message
        string response =
            _resultBuilder.BuildResultMessage(
                results);

        CommunicationLogTextBox.AppendText(
    "\r\n========== ASTM RESULT TO SEND ==========\r\n");

        CommunicationLogTextBox.AppendText(response);

        CommunicationLogTextBox.AppendText(
            "\r\n=========================================\r\n");

        // Send results back
        await _server.SendAsync(response);

        Dispatcher.Invoke(() =>
        {
            CommunicationLogTextBox.AppendText(
                Environment.NewLine);

            CommunicationLogTextBox.AppendText(
                "========== ASTM RESULT SENT ==========\r\n");

            CommunicationLogTextBox.AppendText(
                response + Environment.NewLine);

            CommunicationLogTextBox.ScrollToEnd();
        });
    }

    private void Server_StatusChanged(string status)
    {
        Dispatcher.Invoke(() =>
        {
            StatusTextBlock.Text = status;

            LogTextBox.AppendText(status + Environment.NewLine);
            LogTextBox.ScrollToEnd();
        });
    }
}