using LabAnalyzerConnector.Communication.Abstractions;
using LabAnalyzerConnector.Communication.Exceptions;
using LabAnalyzerConnector.Communication.Serial;
using LabAnalyzerConnector.Communication.Tcp;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Communication.Factories;

public class ConnectionFactory
{
    public IAnalyzerConnection Create(
        Guid analyzerId,
        AnalyzerConfiguration configuration)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(
                nameof(configuration));
        }

        return configuration.ConnectionType switch
        {
            ConnectionType.TcpIp =>
                CreateTcpConnection(
                    analyzerId,
                    configuration),

            ConnectionType.Serial =>
                CreateSerialConnection(
                    analyzerId,
                    configuration),

            _ =>
                throw new ConnectionException(
                    $"Unsupported connection type: " +
                    $"{configuration.ConnectionType}")
        };
    }


    // =========================================================
    // TCP CONNECTION
    // =========================================================

    private static IAnalyzerConnection CreateTcpConnection(
        Guid analyzerId,
        AnalyzerConfiguration configuration)
    {
        if (configuration.Tcp is null)
        {
            throw new ConnectionException(
                "TCP configuration is missing.");
        }

        return configuration.Tcp.Mode switch
        {
            ConnectionMode.Client =>
                new TcpClientConnection(
                    analyzerId,
                    configuration.Tcp,
                    configuration.Direction),

            ConnectionMode.Server =>
                new TcpServerConnection(
                    analyzerId,
                    configuration.Tcp,
                    configuration.Direction),

            _ =>
                throw new ConnectionException(
                    $"Unsupported TCP connection mode: " +
                    $"{configuration.Tcp.Mode}")
        };
    }


    // =========================================================
    // SERIAL CONNECTION
    // =========================================================

    private static IAnalyzerConnection CreateSerialConnection(
        Guid analyzerId,
        AnalyzerConfiguration configuration)
    {
        if (configuration.Serial is null)
        {
            throw new ConnectionException(
                "Serial configuration is missing.");
        }

        return new SerialConnection(
            analyzerId,
            configuration.Serial,
            configuration.Direction);
    }
}