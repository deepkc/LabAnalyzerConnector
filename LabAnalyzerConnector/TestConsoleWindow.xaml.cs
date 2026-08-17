using LabAnalyzerConnector.Core.Configuration;
using System.Windows;
using LabAnalyzerConnector.Application.Services;
using LabAnalyzerConnector.Core.Services;
using LabAnalyzerConnector.Application.Processing;
using System.Windows.Threading;
using LabAnalyzerConnector.Application.Processing.Events;
using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Views;

public partial class TestConsoleWindow : Window
{

    private readonly AnalyzerConnectionCoordinator _connectionCoordinator;

    private readonly IAnalyzerManager _analyzerManager;

    private readonly ProtocolRouter _protocolRouter;
    private readonly ProtocolProcessingCoordinator _processingCoordinator;

    public TestConsoleWindow(
     ProtocolRouter protocolRouter,
     ProtocolProcessingCoordinator processingCoordinator,
     IAnalyzerManager analyzerManager)
    {
        InitializeComponent();

        _protocolRouter = protocolRouter;
        _processingCoordinator = processingCoordinator;
        _analyzerManager = analyzerManager;

        LoadDefaults();
        LoadAnalyzers();

        _processingCoordinator.MessageProcessed +=
            ProcessingCoordinator_MessageProcessed;

        _processingCoordinator.ProcessingError +=
            ProcessingCoordinator_ProcessingError;
    }

    private void LoadDefaults()
    {
        ProtocolComboBox.Items.Add("ASTM");
        ProtocolComboBox.Items.Add("HL7");

        ProtocolComboBox.SelectedIndex = 0;

        DirectionComboBox.Items.Add("Send");
        DirectionComboBox.Items.Add("Receive");

        DirectionComboBox.SelectedIndex = 0;
    }

    private void SendButton_Click(
     object sender,
     RoutedEventArgs e)
    {
        if (AnalyzerComboBox.SelectedItem is not AnalyzerConfiguration analyzer)
        {
            MessageBox.Show("Select an analyzer.");
            return;
        }

        if (string.IsNullOrWhiteSpace(MessageTextBox.Text))
        {
            MessageBox.Show("Paste a message first.");
            return;
        }

        string message = MessageTextBox.Text;

        // ---------------------------------------------------
        // If HL7 -> wrap with MLLP
        // ---------------------------------------------------

        if (ProtocolComboBox.SelectedItem?.ToString() == "HL7")
        {
            char VT = (char)0x0B;
            char FS = (char)0x1C;
            char CR = (char)0x0D;

            message =
                $"{VT}{message}{FS}{CR}";
        }

        Log("------------------------------------------");
        Log($"Injecting {ProtocolComboBox.Text} message...");
        Log("------------------------------------------");

        _protocolRouter.ProcessData(
            analyzer.AnalyzerId,
            message);
    }


    private void LoadAnalyzers()
    {
        AnalyzerComboBox.Items.Clear();

        foreach (AnalyzerConfiguration analyzer in _analyzerManager.GetAnalyzers())
        {
            AnalyzerComboBox.Items.Add(analyzer);
        }

        AnalyzerComboBox.DisplayMemberPath = "Name";

        if (AnalyzerComboBox.Items.Count > 0)
        {
            AnalyzerComboBox.SelectedIndex = 0;
        }
    }
    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        MessageTextBox.Clear();
        LogTextBox.Clear();
    }


    private void ProcessingCoordinator_MessageProcessed(
     object? sender,
     NormalizedMessageProcessedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            Log("");
            Log("========================================");
            Log("MESSAGE PROCESSED");
            Log("========================================");

            Log($"Analyzer        : {e.Message.AnalyzerName}");
            Log($"Patient ID      : {e.Message.PatientId}");
            Log($"Sample ID       : {e.Message.SampleId}");
            Log($"Barcode         : {e.Message.Barcode}");
            Log($"Accession No.   : {e.Message.AccessionNumber}");
            Log($"Received        : {e.Message.ReceivedAtUtc}");

            Log("");
            Log("----------- RESULTS -----------");

            foreach (var result in e.Message.Results)
            {
                Log(
                    $"{result.TestCode,-10} = {result.ResultValue}");
            }

            Log("");
            Log($"Total Results Saved : {e.Message.Results.Count}");

            Log("========================================");
            Log("");
        });
    }

    private void ProcessingCoordinator_ProcessingError(
    object? sender,
    ProtocolProcessingErrorEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            Log("----------------------------------");
            Log("PROCESSING ERROR");
            Log("----------------------------------");

            Log(e.Exception.Message);

            Log("----------------------------------");
        });
    }
    private void LoadSampleButton_Click(object sender, RoutedEventArgs e)
    {
        MessageTextBox.Text =
@"H|\^&|||||||||||P|1
P|1||2002020||midas
O|1|321654||CBC|R
L|1|N";
    }

    private void Log(string message)
    {
        LogTextBox.AppendText(
            $"{DateTime.Now:HH:mm:ss}  {message}{Environment.NewLine}");

        LogTextBox.ScrollToEnd();
    }
}