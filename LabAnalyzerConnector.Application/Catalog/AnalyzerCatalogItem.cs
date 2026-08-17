using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Application.Catalog;

public sealed class AnalyzerCatalogItem
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Manufacturer { get; init; } = "";

    public string Model { get; init; } = "";

    public AnalyzerCategory Category { get; init; }

    public ProtocolType Protocol { get; init; }

    public CommunicationDirection Direction { get; init; }

    public ConnectionType ConnectionType { get; init; }

    public bool SupportsOrders { get; init; }

    public bool SupportsResults { get; init; }

    public bool SupportsQc { get; init; }

    public string DefaultIp { get; init; } = "";

    public int DefaultPort { get; init; }

    public string DefaultComPort { get; init; } = "";

    public int DefaultBaudRate { get; init; }

    public string Notes { get; init; } = "";


    public string ProfileVersion { get; init; } = "1.0";

    public string DefaultProtocolVersion { get; init; } = "";


    public int DefaultDataBits { get; init; } = 8;

    public string DefaultParity { get; init; } = "None";

    public string DefaultStopBits { get; init; } = "One";
}