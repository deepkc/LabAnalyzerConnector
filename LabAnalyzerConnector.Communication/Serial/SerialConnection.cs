using System.IO.Ports;
using System.Text;
using LabAnalyzerConnector.Communication.Abstractions;
using LabAnalyzerConnector.Communication.Exceptions;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Communication.Serial;

public sealed class SerialConnection : IAnalyzerConnection
{
    private readonly Guid _analyzerId;
    private readonly SerialConfiguration _configuration;
    public CommunicationDirection Direction { get; }
    private SerialPort? _serialPort;

    public Guid AnalyzerId => _analyzerId;

    public ConnectionStatus Status { get; private set; } =
        ConnectionStatus.Disconnected;

    public event EventHandler<ConnectionStatus>? StatusChanged;

    public event EventHandler<string>? DataReceived;

    public event EventHandler<Exception>? ErrorOccurred;

    public SerialConnection(
     Guid analyzerId,
     SerialConfiguration configuration,
     CommunicationDirection direction)
    {
        _analyzerId = analyzerId;

        _configuration =
            configuration
            ?? throw new ArgumentNullException(
                nameof(configuration));

        Direction =
            direction;
    }

    public Task ConnectAsync(
        CancellationToken cancellationToken = default)
    {
        if (Status == ConnectionStatus.Connected)
        {
            return Task.CompletedTask;
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            SetStatus(
                ConnectionStatus.Connecting);

            _serialPort = new SerialPort
            {
                PortName =
                    _configuration.PortName,

                BaudRate =
                    _configuration.BaudRate,

                DataBits =
                    _configuration.DataBits,

                Parity =
                    _configuration.Parity,

                StopBits =
                    _configuration.StopBits,

                Handshake =
                    _configuration.Handshake,

                DtrEnable =
                    _configuration.DtrEnable,

                RtsEnable =
                    _configuration.RtsEnable,

                ReadTimeout =
                    _configuration.ReadTimeoutMilliseconds,

                WriteTimeout =
                    _configuration.WriteTimeoutMilliseconds,

                Encoding =
                    Encoding.ASCII
            };

            _serialPort.DataReceived +=
                SerialPort_DataReceived;

            _serialPort.ErrorReceived +=
                SerialPort_ErrorReceived;

            _serialPort.Open();

            SetStatus(
                ConnectionStatus.Connected);

            return Task.CompletedTask;
        }
        catch (OperationCanceledException)
        {
            SetStatus(
                ConnectionStatus.Disconnected);

            throw;
        }
        catch (Exception ex)
        {
            SetStatus(
                ConnectionStatus.Error);

            var connectionException =
                new ConnectionException(
                    $"Failed to open serial port {_configuration.PortName}.",
                    ex);

            ErrorOccurred?.Invoke(
                this,
                connectionException);

            throw connectionException;
        }
    }

    public Task DisconnectAsync()
    {
        try
        {
            if (_serialPort is null)
            {
                return Task.CompletedTask;
            }

            _serialPort.DataReceived -=
                SerialPort_DataReceived;

            _serialPort.ErrorReceived -=
                SerialPort_ErrorReceived;

            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }

            _serialPort.Dispose();

            _serialPort = null;

            SetStatus(
                ConnectionStatus.Disconnected);
        }
        catch (Exception ex)
        {
            SetStatus(
                ConnectionStatus.Error);

            ErrorOccurred?.Invoke(
                this,
                ex);
        }

        return Task.CompletedTask;
    }

    public Task SendAsync(
        string data,
        CancellationToken cancellationToken = default)
    {
        if (_serialPort is null ||
            !_serialPort.IsOpen)
        {
            throw new ConnectionException(
                "Serial connection is not active.");
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            _serialPort.Write(data);

            return Task.CompletedTask;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            SetStatus(
                ConnectionStatus.Error);

            ErrorOccurred?.Invoke(
                this,
                ex);

            throw new ConnectionException(
                "Failed to send data through serial port.",
                ex);
        }
    }

    private void SerialPort_DataReceived(
        object sender,
        SerialDataReceivedEventArgs e)
    {
        try
        {
            if (_serialPort is null ||
                !_serialPort.IsOpen)
            {
                return;
            }

            string data =
                _serialPort.ReadExisting();

            if (!string.IsNullOrEmpty(data))
            {
                DataReceived?.Invoke(
                    this,
                    data);
            }
        }
        catch (Exception ex)
        {
            SetStatus(
                ConnectionStatus.Error);

            ErrorOccurred?.Invoke(
                this,
                ex);
        }
    }

    private void SerialPort_ErrorReceived(
        object sender,
        SerialErrorReceivedEventArgs e)
    {
        var exception =
            new ConnectionException(
                $"Serial communication error on {_configuration.PortName}: {e.EventType}");

        SetStatus(
            ConnectionStatus.Error);

        ErrorOccurred?.Invoke(
            this,
            exception);
    }

    private void SetStatus(
        ConnectionStatus status)
    {
        if (Status == status)
        {
            return;
        }

        Status = status;

        StatusChanged?.Invoke(
            this,
            status);
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }
}