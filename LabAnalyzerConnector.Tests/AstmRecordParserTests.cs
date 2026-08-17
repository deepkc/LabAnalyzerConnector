using LabAnalyzerConnector.Protocols.ASTM;
using LabAnalyzerConnector.Protocols.ASTM.Models;

namespace LabAnalyzerConnector.Tests;

public sealed class AstmRecordParserTests
{
    [Fact]
    public void Parse_Mek7300Message_ShouldExtractPatientSampleAndResults()
    {
        // Arrange
        string message =
            "H|\\^&|||MEK-7300^1^01592^V02-14^V04-02|||||||P|E1394-97|20230223125341\r" +
            "P|1|||0279070002|&H&&N&||||||||&H&&N&|&H&&N&|||||||||||&H&&N&\r" +
            "O|1|0279070002^M^1|25134||||||||||||BLOOD^019|||||||20230218091844|||F\r" +
            "R|1|^^2A0100000019301^WBC^JC10|10.7|10e3/uL^1^1|4.0-9.0|H|||||||MEK-7300\r" +
            "R|2|^^2A0200000019301^RBC^JC10|4.75|10e6/uL^1^1|3.76-5.70||||||||MEK-7300\r" +
            "R|3|^^2A0300000019301^HGB^JC10|11.8|g/dL^1^0|12.0-18.0|L|||||||MEK-7300\r" +
            "L|1";

        var parser =
            new AstmRecordParser();

        // Act
        AstmMessage result =
            parser.Parse(message);

        // Assert - Records
        Assert.NotNull(result.Header);
        Assert.NotNull(result.Patient);
        Assert.NotNull(result.Order);

        // Patient
        Assert.Equal(
            "0279070002",
            result.Patient!.PatientId);

        // Order
        Assert.Equal(
            "0279070002",
            result.Order!.SampleId);

        // Results
        Assert.Equal(
            3,
            result.Results.Count);

        // WBC
        AstmResultRecord wbc =
            result.Results[0];

        Assert.Equal("WBC", wbc.TestCode);
        Assert.Equal("10.7", wbc.Value);
        Assert.Equal("10e3/uL^1^1", wbc.Units);
        Assert.Equal("4.0-9.0", wbc.ReferenceRange);
        Assert.Equal("H", wbc.AbnormalFlag);

        // RBC
        AstmResultRecord rbc =
            result.Results[1];

        Assert.Equal("RBC", rbc.TestCode);
        Assert.Equal("4.75", rbc.Value);

        // HGB
        AstmResultRecord hgb =
            result.Results[2];

        Assert.Equal("HGB", hgb.TestCode);
        Assert.Equal("11.8", hgb.Value);
        Assert.Equal("g/dL^1^0", hgb.Units);
        Assert.Equal("12.0-18.0", hgb.ReferenceRange);
        Assert.Equal("L", hgb.AbnormalFlag);
    }
}