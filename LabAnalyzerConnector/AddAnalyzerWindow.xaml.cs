using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using LabAnalyzerConnector.Application.Catalog;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Enums;
using LabAnalyzerConnector.Application.Profiles;
using LabAnalyzerConnector.Application.Services;
using LabAnalyzerConnector.Core.Configuration;

namespace LabAnalyzerConnector;

public partial class AddAnalyzerWindow : Window
{
    // =========================================================
    // CREATED CONFIGURATION
    // =========================================================
    private readonly IAnalyzerCatalogService _catalogService =
     new AnalyzerCatalogService();
    private readonly IAnalyzerProfileRepository _profileRepository;
    private readonly AnalyzerConnectionTestService _connectionTestService;
    public AnalyzerConfiguration? CreatedConfiguration
    {
        get;
        private set;
    }

    public AnalyzerConfiguration? ExistingConfiguration
    {
        get;
        private set;
    }

    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public AddAnalyzerWindow(
     IAnalyzerCatalogService catalogService,
     AnalyzerConnectionTestService connectionTestService)
    {
        InitializeComponent();

        _catalogService = catalogService;
        _connectionTestService = connectionTestService;

        InitializeDropdowns();

        LoadAvailableComPorts();

        ConnectionTypeComboBox.SelectionChanged +=
            ConnectionTypeComboBox_SelectionChanged;

        ConnectionModeComboBox.SelectionChanged +=
            ConnectionModeComboBox_SelectionChanged;

        ManufacturerComboBox.SelectionChanged +=
            ManufacturerComboBox_SelectionChanged;

        ModelComboBox.SelectionChanged +=
            ModelComboBox_SelectionChanged;

        UpdateConnectionPanels();

        LoadManufacturers();
    }

    public AddAnalyzerWindow(
    IAnalyzerCatalogService catalogService,
    AnalyzerConnectionTestService connectionTestService,
    AnalyzerConfiguration existingConfiguration)
    : this(catalogService, connectionTestService)
    {
        ExistingConfiguration = existingConfiguration;

        LoadExistingConfiguration(existingConfiguration);
    }

    private void LoadManufacturers()
    {
        ManufacturerComboBox.ItemsSource =
            _catalogService.GetManufacturers();
    }
    // =========================================================
    // INITIALIZE DROPDOWNS
    // =========================================================

    private void InitializeDropdowns()
    {
        // =====================================================
        // PROTOCOL
        // =====================================================

        ProtocolComboBox.ItemsSource =
            Enum.GetValues<ProtocolType>();

        ProtocolComboBox.SelectedIndex =
            0;


        // =====================================================
        // COMMUNICATION DIRECTION
        // =====================================================

        CommunicationDirectionComboBox.ItemsSource =
            Enum.GetValues<CommunicationDirection>();

        CommunicationDirectionComboBox.SelectedItem =
            CommunicationDirection.Bidirectional;


        // =====================================================
        // CONNECTION TYPE
        // =====================================================

        ConnectionTypeComboBox.ItemsSource =
            Enum.GetValues<ConnectionType>();

        ConnectionTypeComboBox.SelectedIndex =
            0;


        // =====================================================
        // TCP CONNECTION MODE
        // =====================================================

        ConnectionModeComboBox.ItemsSource =
            Enum.GetValues<ConnectionMode>();

        ConnectionModeComboBox.SelectedItem =
            ConnectionMode.Client;


        // =====================================================
        // SERIAL BAUD RATE
        // =====================================================

        BaudRateComboBox.ItemsSource =
            new[]
            {
                9600,
                19200,
                38400,
                57600,
                115200
            };

        BaudRateComboBox.SelectedItem =
            9600;


        // =====================================================
        // SERIAL DATA BITS
        // =====================================================

        DataBitsComboBox.ItemsSource =
            new[]
            {
                5,
                6,
                7,
                8
            };

        DataBitsComboBox.SelectedItem =
            8;


        // =====================================================
        // SERIAL PARITY
        // =====================================================

        ParityComboBox.ItemsSource =
            Enum.GetValues<Parity>();

        ParityComboBox.SelectedItem =
            Parity.None;


        // =====================================================
        // SERIAL STOP BITS
        // =====================================================

        StopBitsComboBox.ItemsSource =
            Enum.GetValues<StopBits>();

        StopBitsComboBox.SelectedItem =
            StopBits.One;

        ManufacturerComboBox.ItemsSource =
    _catalogService.GetManufacturers();
    }


    private void LoadExistingConfiguration(
    AnalyzerConfiguration configuration)
    {

        CreatedConfiguration = configuration;
        NameTextBox.Text =
            configuration.Name;

        ManufacturerComboBox.SelectedItem =
            configuration.Manufacturer;

        ModelComboBox.SelectedItem =
            configuration.Model;

        SerialNumberTextBox.Text =
            configuration.SerialNumber;

        IsEnabledCheckBox.IsChecked =
            configuration.IsEnabled;

        AutoConnectCheckBox.IsChecked =
            configuration.AutoConnect;

        AutoReconnectCheckBox.IsChecked =
            configuration.AutoReconnect;

        ConnectionTypeComboBox.SelectedItem =
            configuration.ConnectionType;

        CommunicationDirectionComboBox.SelectedItem =
            configuration.Direction;

        ProtocolComboBox.SelectedItem =
            configuration.Protocol.ProtocolType;

        ProtocolVersionTextBox.Text =
            configuration.Protocol.ProtocolVersion;

        if (configuration.ConnectionType ==
            ConnectionType.Serial &&
            configuration.Serial is not null)
        {
            ComPortComboBox.Text =
                configuration.Serial.PortName;

            BaudRateComboBox.SelectedItem =
                configuration.Serial.BaudRate;

            DataBitsComboBox.SelectedItem =
                configuration.Serial.DataBits;

            ParityComboBox.SelectedItem =
                configuration.Serial.Parity;

            StopBitsComboBox.SelectedItem =
                configuration.Serial.StopBits;
        }

        if (configuration.ConnectionType ==
            ConnectionType.TcpIp &&
            configuration.Tcp is not null)
        {
            ConnectionModeComboBox.SelectedItem =
                configuration.Tcp.Mode;

            if (configuration.Tcp.Mode ==
                ConnectionMode.Client)
            {
                RemoteIpAddressTextBox.Text =
                    configuration.Tcp.RemoteIpAddress;

                RemotePortTextBox.Text =
                    configuration.Tcp.RemotePort.ToString();
            }
            else
            {
                LocalIpAddressTextBox.Text =
                    configuration.Tcp.LocalIpAddress;

                LocalPortTextBox.Text =
                    configuration.Tcp.LocalPort.ToString();
            }
        }

        UpdateConnectionPanels();
    }

    // =========================================================
    // CONNECTION TYPE CHANGED
    // =========================================================

    private void ConnectionTypeComboBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        UpdateConnectionPanels();
    }


    // =========================================================
    // TCP CONNECTION MODE CHANGED
    // =========================================================

    private void ConnectionModeComboBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        UpdateTcpModePanels();
    }


    // =========================================================
    // UPDATE CONNECTION PANELS
    // =========================================================

    private void UpdateConnectionPanels()
    {
        if (ConnectionTypeComboBox.SelectedItem
            is not ConnectionType connectionType)
        {
            return;
        }


        bool isTcp =
            connectionType ==
            ConnectionType.TcpIp;


        bool isSerial =
            connectionType ==
            ConnectionType.Serial;


        TcpConfigurationPanel.Visibility =
            isTcp
                ? Visibility.Visible
                : Visibility.Collapsed;


        SerialConfigurationPanel.Visibility =
            isSerial
                ? Visibility.Visible
                : Visibility.Collapsed;


        if (isTcp)
        {
            UpdateTcpModePanels();
        }
    }


    // =========================================================
    // UPDATE TCP MODE PANELS
    // =========================================================

    private void UpdateTcpModePanels()
    {
        if (ConnectionModeComboBox.SelectedItem
            is not ConnectionMode connectionMode)
        {
            return;
        }


        bool isClient =
            connectionMode ==
            ConnectionMode.Client;


        TcpClientPanel.Visibility =
            isClient
                ? Visibility.Visible
                : Visibility.Collapsed;


        TcpServerPanel.Visibility =
            isClient
                ? Visibility.Collapsed
                : Visibility.Visible;
    }

    private AnalyzerConfiguration? BuildConfiguration()
    {
        return null;
    }


    // =========================================================
    // SAVE ANALYZER
    // =========================================================

    private void SaveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            // =================================================
            // VALIDATE ANALYZER NAME
            // =================================================

            if (ComPortComboBox.SelectedItem is not string portName)
            {
                ShowValidationError(
                    "Please select a COM port.");

                return;
            }


            // =================================================
            // VALIDATE PROTOCOL
            // =================================================

            if (ProtocolComboBox.SelectedItem
                is not ProtocolType protocolType)
            {
                ShowValidationError(
                    "Please select a protocol.");

                return;
            }


            // =================================================
            // VALIDATE COMMUNICATION DIRECTION
            // =================================================

            if (CommunicationDirectionComboBox.SelectedItem
                is not CommunicationDirection selectedDirection)
            {
                ShowValidationError(
                    "Please select a communication direction.");

                return;
            }


            // =================================================
            // VALIDATE CONNECTION TYPE
            // =================================================

            if (ConnectionTypeComboBox.SelectedItem
                is not ConnectionType connectionType)
            {
                ShowValidationError(
                    "Please select a connection type.");

                return;
            }


            // =================================================
            // CREATE BASE CONFIGURATION
            // =================================================


            var configuration =
     new AnalyzerConfiguration
     {
         Id =
             ExistingConfiguration?.Id
             ?? Guid.NewGuid(),

         AnalyzerId =
             ExistingConfiguration?.AnalyzerId
             ?? Guid.NewGuid(),

         Name =
             NameTextBox.Text.Trim(),

         Manufacturer =
             ManufacturerComboBox.Text.Trim(),

         Model =
             ModelComboBox.Text.Trim(),

         SerialNumber =
             SerialNumberTextBox.Text.Trim(),

         IsEnabled =
             IsEnabledCheckBox.IsChecked
             == true,

         AutoConnect =
             AutoConnectCheckBox.IsChecked
             == true,

         AutoReconnect =
             AutoReconnectCheckBox.IsChecked
             == true,

         ConnectionType =
             connectionType,

         Direction =
             selectedDirection,

         Protocol =
             new ProtocolConfiguration
             {
                 ProtocolType =
                     protocolType,

                 ProtocolVersion =
                     ProtocolVersionTextBox
                         .Text
                         .Trim()
             },

         CreatedAtUtc =
             DateTime.UtcNow,

         UpdatedAtUtc =
             DateTime.UtcNow
     };


            // =================================================
            // TCP/IP CONFIGURATION
            // =================================================

            if (connectionType ==
                ConnectionType.TcpIp)
            {
                if (ConnectionModeComboBox.SelectedItem
                    is not ConnectionMode connectionMode)
                {
                    ShowValidationError(
                        "Please select a TCP connection mode.");

                    return;
                }


                var tcp =
                    new TcpConfiguration
                    {
                        Mode =
                            connectionMode
                    };


                // =================================================
                // TCP CLIENT
                // =================================================

                if (connectionMode ==
                    ConnectionMode.Client)
                {
                    if (string.IsNullOrWhiteSpace(
                            RemoteIpAddressTextBox.Text))
                    {
                        ShowValidationError(
                            "Remote analyzer IP address is required.");

                        RemoteIpAddressTextBox.Focus();

                        return;
                    }


                    if (!int.TryParse(
                            RemotePortTextBox.Text,
                            out int remotePort) ||
                        remotePort < 1 ||
                        remotePort > 65535)
                    {
                        ShowValidationError(
                            "Please enter a valid remote TCP port between 1 and 65535.");

                        RemotePortTextBox.Focus();

                        return;
                    }


                    tcp.RemoteIpAddress =
                        RemoteIpAddressTextBox
                            .Text
                            .Trim();


                    tcp.RemotePort =
                        remotePort;
                }


                // =================================================
                // TCP SERVER
                // =================================================

                else
                {
                    if (string.IsNullOrWhiteSpace(
                            LocalIpAddressTextBox.Text))
                    {
                        ShowValidationError(
                            "Local IP address is required.");

                        LocalIpAddressTextBox.Focus();

                        return;
                    }


                    if (!int.TryParse(
                            LocalPortTextBox.Text,
                            out int localPort) ||
                        localPort < 1 ||
                        localPort > 65535)
                    {
                        ShowValidationError(
                            "Please enter a valid local TCP port between 1 and 65535.");

                        LocalPortTextBox.Focus();

                        return;
                    }


                    tcp.LocalIpAddress =
                        LocalIpAddressTextBox
                            .Text
                            .Trim();


                    tcp.LocalPort =
                        localPort;
                }


                configuration.Tcp =
                    tcp;
            }


            // =================================================
            // SERIAL CONFIGURATION
            // =================================================

            if (connectionType ==
                ConnectionType.Serial)
            {
                if (string.IsNullOrWhiteSpace(
                       ComPortComboBox.Text))
                {
                    ShowValidationError(
                        "COM port is required.");

                    ComPortComboBox.Focus();

                    return;
                }


                if (BaudRateComboBox.SelectedItem
                    is not int baudRate)
                {
                    ShowValidationError(
                        "Please select a baud rate.");

                    return;
                }


                if (DataBitsComboBox.SelectedItem
                    is not int dataBits)
                {
                    ShowValidationError(
                        "Please select data bits.");

                    return;
                }


                if (ParityComboBox.SelectedItem
                    is not Parity parity)
                {
                    ShowValidationError(
                        "Please select parity.");

                    return;
                }


                if (StopBitsComboBox.SelectedItem
                    is not StopBits stopBits)
                {
                    ShowValidationError(
                        "Please select stop bits.");

                    return;
                }


                configuration.Serial =
                    new SerialConfiguration
                    {
                        PortName = portName,

                        BaudRate =
                            baudRate,

                        DataBits =
                            dataBits,

                        Parity =
                            parity,

                        StopBits =
                            stopBits
                    };
            }


            // =================================================
            // RETURN CONFIGURATION
            // =================================================

            CreatedConfiguration =
                configuration;


            DialogResult =
                true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to create analyzer configuration.\n\n{ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    // =========================================================
    // CANCEL
    // =========================================================

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult =
            false;
    }
    private void ManufacturerComboBox_SelectionChanged(
    object sender,
    SelectionChangedEventArgs e)
    {
        if (ManufacturerComboBox.SelectedItem?.ToString() == "Custom")
        {
            return;
        }

        if (ManufacturerComboBox.SelectedItem is not string manufacturer)
            return;

        if (manufacturer == "Custom")
        {
            EnableCustomMode();

            ModelComboBox.ItemsSource = null;
            ModelComboBox.Text = "";
            ModelComboBox.IsEditable = true;

            return;
        }

        DisableCustomMode();

        ModelComboBox.IsEditable = false;

        ModelComboBox.ItemsSource =
            _catalogService.GetModels(manufacturer);

        ModelComboBox.SelectedIndex = -1;
    }

    private void EnableCustomMode()
    {
        ModelComboBox.IsEditable = true;

        ProtocolComboBox.IsEnabled = true;
        CommunicationDirectionComboBox.IsEnabled = true;
        ConnectionTypeComboBox.IsEnabled = true;

        BaudRateComboBox.IsEnabled = true;
        DataBitsComboBox.IsEnabled = true;
        ParityComboBox.IsEnabled = true;
        StopBitsComboBox.IsEnabled = true;

        ProtocolVersionTextBox.IsEnabled = true;

        NameTextBox.Clear();
        ProtocolVersionTextBox.Clear();

      
    }


    private void LoadAvailableComPorts()
    {
        var ports = System.IO.Ports.SerialPort
            .GetPortNames()
            .OrderBy(x => x)
            .ToList();

        // Always include COM1 so catalog defaults work,
        // even if no physical COM1 exists.
        if (!ports.Contains("COM1"))
        {
            ports.Insert(0, "COM1");
        }

        ComPortComboBox.ItemsSource = ports;

        if (ports.Count > 0)
        {
            ComPortComboBox.SelectedIndex = 0;
        }
    }
    private void DisableCustomMode()
    {
        ModelComboBox.IsEditable = false;

        ProtocolComboBox.IsEnabled = false;
        CommunicationDirectionComboBox.IsEnabled = false;
        ConnectionTypeComboBox.IsEnabled = false;

        BaudRateComboBox.IsEnabled = false;
        DataBitsComboBox.IsEnabled = false;
        ParityComboBox.IsEnabled = false;
        StopBitsComboBox.IsEnabled = false;

        ProtocolVersionTextBox.IsEnabled = false;
    }

    private void ModelComboBox_SelectionChanged(
     object sender,
     SelectionChangedEventArgs e)
    {
        if (ManufacturerComboBox.SelectedItem is not string manufacturer)
            return;

        if (ModelComboBox.SelectedItem is not string model)
            return;

        var analyzer =
            _catalogService.Get(manufacturer, model);

        if (analyzer == null)
            return;

        ProfileProtocolTextBox.Text =
    analyzer.Protocol.ToString();

        ProfileDirectionTextBox.Text =
            analyzer.Direction.ToString();

        ProfileConnectionTypeTextBox.Text =
            analyzer.ConnectionType.ToString();

        ProfileComPortTextBox.Text =
            analyzer.DefaultComPort;

        ProfileBaudRateTextBox.Text =
            analyzer.DefaultBaudRate.ToString();

        UpdateProfileSummary(analyzer);

        if (analyzer == null)
            return;

        ProtocolComboBox.SelectedItem =
            analyzer.Protocol;

        ProtocolVersionTextBox.Text =
    analyzer.DefaultProtocolVersion;

        if (ComPortComboBox.Items.Contains(analyzer.DefaultComPort))
        {
            ComPortComboBox.SelectedItem = analyzer.DefaultComPort;
        }
        else
        {
            ComPortComboBox.Items.Add(analyzer.DefaultComPort);
            ComPortComboBox.SelectedItem = analyzer.DefaultComPort;
        }

        BaudRateComboBox.SelectedItem =
            analyzer.DefaultBaudRate;

        DataBitsComboBox.SelectedItem =
    analyzer.DefaultDataBits;

        ParityComboBox.SelectedItem =
            Enum.Parse<Parity>(
                analyzer.DefaultParity);

        StopBitsComboBox.SelectedItem =
            Enum.Parse<StopBits>(
                analyzer.DefaultStopBits);

        CommunicationDirectionComboBox.SelectedItem =
            analyzer.Direction;

        ConnectionTypeComboBox.SelectedItem =
            analyzer.ConnectionType;

        // =====================================
        // Lock profile-controlled fields
        // =====================================

        ProtocolComboBox.IsEnabled = false;

        ProtocolVersionTextBox.IsReadOnly = true;

        CommunicationDirectionComboBox.IsEnabled = false;

        ConnectionTypeComboBox.IsEnabled = false;

        BaudRateComboBox.IsEnabled = false;

        DataBitsComboBox.IsEnabled = false;

        ParityComboBox.IsEnabled = false;

        StopBitsComboBox.IsEnabled = false;

        SetProfileFieldsLocked(false);

        // =====================================
        // Auto-generate analyzer name
        // =====================================

        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            string manufacturerShort =
                analyzer.Manufacturer
                    .Replace("Diagnostics", "")
                    .Trim();

            NameTextBox.Text =
                $"{manufacturerShort} {analyzer.Model}";
        }

        // Default communication
        if (analyzer.ConnectionType == ConnectionType.Serial)
        {
            ComPortComboBox.Text =
                analyzer.DefaultComPort;

            BaudRateComboBox.SelectedItem =
                analyzer.DefaultBaudRate;
        }
        else
        {
            RemoteIpAddressTextBox.Text =
                analyzer.DefaultIp;

            RemotePortTextBox.Text =
                analyzer.DefaultPort.ToString();
        }

        // -------- Profile Information --------

        CategoryTextBox.Text =
            analyzer.Category.ToString();

        ProfileVersionTextBox.Text =
            analyzer.ProfileVersion;

        SupportsOrdersCheckBox.IsChecked =
            analyzer.SupportsOrders;

        SupportsResultsCheckBox.IsChecked =
            analyzer.SupportsResults;

        SupportsQcCheckBox.IsChecked =
            analyzer.SupportsQc;

        NotesTextBox.Text =
            analyzer.Notes;
    }

    // =========================================================
    // VALIDATION ERROR
    // =========================================================

    private static void ShowValidationError(
        string message)
    {
        MessageBox.Show(
            message,
            "Validation Error",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
 
    }

    private void SetProfileFieldsLocked(bool locked)
    {
        ProtocolComboBox.IsEnabled = !locked;

        ProtocolVersionTextBox.IsReadOnly = locked;

        CommunicationDirectionComboBox.IsEnabled = !locked;

        ConnectionTypeComboBox.IsEnabled = !locked;

        BaudRateComboBox.IsEnabled = !locked;

        DataBitsComboBox.IsEnabled = !locked;

        ParityComboBox.IsEnabled = !locked;

        StopBitsComboBox.IsEnabled = !locked;
    }

    private void ApplyProfile(
    AnalyzerProfile profile)
    {
        ProtocolComboBox.SelectedItem =
            ProtocolType.Astm;

        CommunicationDirectionComboBox.SelectedItem =
            CommunicationDirection.Bidirectional;

        ConnectionTypeComboBox.SelectedItem =
            ConnectionType.Serial;

        BaudRateComboBox.SelectedItem =
            profile.Communication.DefaultBaudRate;

        DataBitsComboBox.SelectedItem =
            profile.Communication.DefaultDataBits;

        ParityComboBox.SelectedItem =
            Enum.Parse<Parity>(
                profile.Communication.DefaultParity);

        StopBitsComboBox.SelectedItem =
            Enum.Parse<StopBits>(
                profile.Communication.DefaultStopBits);

        AutoReconnectCheckBox.IsChecked =
            profile.Communication.AutoReconnect;
    }

    private async void TestConnectionButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        ConnectionStatusText.Text = "Testing...";
        ConnectionStatusText.Foreground =
            System.Windows.Media.Brushes.DarkBlue;

        TestConnectionButton.IsEnabled = false;

        try
        {
            // Temporary simulation
            var configuration = new AnalyzerConfiguration
            {

                Id =
                    ExistingConfiguration?.Id
                    ?? Guid.NewGuid(),

                                AnalyzerId =
                    ExistingConfiguration?.AnalyzerId
                    ?? Guid.NewGuid(),

                Name = NameTextBox.Text.Trim(),

                Manufacturer = ManufacturerComboBox.Text,

                Model = ModelComboBox.Text,

                ConnectionType =
         (ConnectionType)ConnectionTypeComboBox.SelectedItem,

                Direction =
         (CommunicationDirection)CommunicationDirectionComboBox.SelectedItem,

                AutoReconnect =
    AutoReconnectCheckBox.IsChecked == true,

                Protocol = new ProtocolConfiguration
                {
                    ProtocolType =
             (ProtocolType)ProtocolComboBox.SelectedItem,

                    ProtocolVersion =
             ProtocolVersionTextBox.Text.Trim()
                }
            };

            if (configuration.ConnectionType == ConnectionType.Serial)
            {
                configuration.Serial = new SerialConfiguration
                {
                    PortName = ComPortComboBox.Text.Trim(),

                    BaudRate = (int)BaudRateComboBox.SelectedItem,

                    DataBits = (int)DataBitsComboBox.SelectedItem,

                    Parity = (Parity)ParityComboBox.SelectedItem,

                    StopBits = (StopBits)StopBitsComboBox.SelectedItem
                };
            }
            else
            {
                var mode =
                    (ConnectionMode)ConnectionModeComboBox.SelectedItem;

                configuration.Tcp = new TcpConfiguration
                {
                    Mode = mode
                };

                if (mode == ConnectionMode.Client)
                {
                    configuration.Tcp.RemoteIpAddress =
                        RemoteIpAddressTextBox.Text.Trim();

                    configuration.Tcp.RemotePort =
                        int.Parse(RemotePortTextBox.Text);
                }
                else
                {
                    configuration.Tcp.LocalIpAddress =
                        LocalIpAddressTextBox.Text.Trim();

                    configuration.Tcp.LocalPort =
                        int.Parse(LocalPortTextBox.Text);
                }
            }

            bool success =
                await _connectionTestService.TestAsync(configuration);

            if (success)
            {
                ConnectionStatusText.Text =
                    "Connection Successful";

                ConnectionStatusText.Foreground =
                    System.Windows.Media.Brushes.Green;
            }
            else
            {
                ConnectionStatusText.Text =
                    "Connection Failed";

                ConnectionStatusText.Foreground =
                    System.Windows.Media.Brushes.Red;
            }
        }
        catch
        {
            ConnectionStatusText.Text =
                "Connection Failed";

            ConnectionStatusText.Foreground =
                System.Windows.Media.Brushes.Red;
        }
        finally
        {
            TestConnectionButton.IsEnabled = true;
        }
    }

    private void UpdateProfileSummary(
    AnalyzerCatalogItem analyzer)
    {
        ProfileCategoryText.Text =
            $"Category: {analyzer.Category}";

        ProfileProtocolText.Text =
            $"Protocol: {analyzer.Protocol}";

        ProfileDirectionText.Text =
            $"Communication: {analyzer.Direction}";

        ProfileConnectionText.Text =
            $"Connection: {analyzer.ConnectionType}";

        ProfileOrdersText.Text =
            $"Supports Orders: {(analyzer.SupportsOrders ? "Yes" : "No")}";

        ProfileResultsText.Text =
            $"Supports Results: {(analyzer.SupportsResults ? "Yes" : "No")}";

        ProfileQcText.Text =
            $"Supports QC: {(analyzer.SupportsQc ? "Yes" : "No")}";

        ProfileDefaultComText.Text =
            $"Default COM: {analyzer.DefaultComPort}";

        ProfileDefaultBaudText.Text =
            $"Default Baud: {analyzer.DefaultBaudRate}";

        ProfileNotesText.Text =
            $"Notes: {analyzer.Notes}";
    }
}