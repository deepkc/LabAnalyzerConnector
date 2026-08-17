using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Application.Catalog.Manufacturers;

public static class AbbottCatalog
{
    public static IEnumerable<AnalyzerCatalogItem> Get()
    {
        yield return new AnalyzerCatalogItem
        {
            Manufacturer = "Abbott Diagnostics",

            Model = "Architect c4000",

            Category = AnalyzerCategory.ClinicalChemistry,

            Protocol = ProtocolType.Astm,

            Direction = CommunicationDirection.Bidirectional,

            ConnectionType = ConnectionType.Serial,

            DefaultProtocolVersion = "ASTM E1381/E1394",

            SupportsOrders = true,

            SupportsResults = true,

            SupportsQc = true,

            DefaultBaudRate = 9600,

            DefaultComPort = "COM1",

            Notes = "Abbott Architect Clinical Chemistry Analyzer",

            DefaultDataBits = 8,

            DefaultParity = "None",

            DefaultStopBits = "One",

        };
    }
}