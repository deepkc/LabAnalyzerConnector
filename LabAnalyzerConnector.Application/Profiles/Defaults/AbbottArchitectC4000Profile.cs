using System;

namespace LabAnalyzerConnector.Application.Profiles.Defaults;

public static class AbbottArchitectC4000Profile
{
    public static AnalyzerProfile Create()
    {
        return new AnalyzerProfile
        {
            Id = Guid.NewGuid(),

            Manufacturer = "Abbott Diagnostics",

            Model = "Abbott Architect c4000",

            Communication = new AnalyzerCommunicationProfile
            {
                DefaultBaudRate = 9600,
                DefaultDataBits = 8,
                DefaultParity = "None",
                DefaultStopBits = "One",
                AutoReconnect = true,
                ReconnectIntervalSeconds = 5,
                ReadTimeoutMilliseconds = 30000,
                WriteTimeoutMilliseconds = 30000
            },

            Protocol = new AnalyzerProtocolProfile
            {
                RequiresAck = true,
                UsesChecksum = true,
                UsesEnqEot = true,
                SupportsOrderQuery = true,
                SupportsResults = true,
                RetryCount = 3,
                AckTimeoutMilliseconds = 5000
            },

            Parsing = new AnalyzerParsingProfile
            {
                SampleIdField = "O.3",
                PatientIdField = "P.3",
                TestCodeField = "R.2",
                ResultField = "R.3",
                UnitsField = "R.4",
                FlagsField = "R.6"
            },

            Orders = new AnalyzerOrderProfile
            {
                SupportsOrderDownload = true,
                SupportsHostQuery = true,
                SupportsBarcodeQuery = true
            },

            Results = new AnalyzerResultProfile
            {
                SupportsResultCorrection = true,
                SupportsDeltaResults = false,
                SupportsReferenceRanges = true,
                SupportsResultFlags = true
            }
        };
    }
}