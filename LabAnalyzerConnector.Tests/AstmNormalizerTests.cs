using LabAnalyzerConnector.Application.Normalization;
using LabAnalyzerConnector.Domain.Entities;
using LabAnalyzerConnector.Protocols.ASTM;
using LabAnalyzerConnector.Protocols.ASTM.Models;

namespace LabAnalyzerConnector.Tests;

public sealed class AstmNormalizerTests
{
    [Fact]
    public void Normalize_Mek7300Message_ShouldCreateNormalizedLabMessage()
    {
        // =====================================================
        // Arrange
        // =====================================================

        Guid analyzerId =
            Guid.NewGuid();

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

        AstmMessage parsedMessage =
            parser.Parse(
                message);

        var normalizer =
            new AstmNormalizer();


        // =====================================================
        // Act
        // =====================================================

        IEnumerable<NormalizedLabMessage> normalizedMessages =
            normalizer.Normalize(
                analyzerId,
                parsedMessage);

        NormalizedLabMessage normalizedMessage =
            normalizedMessages.Single();


        // =====================================================
        // Assert - Main Message
        // =====================================================

        Assert.NotEqual(
            Guid.Empty,
            normalizedMessage.Id);

        Assert.Equal(
            analyzerId,
            normalizedMessage.AnalyzerId);

        Assert.Equal(
            "0279070002",
            normalizedMessage.PatientId);

        Assert.Equal(
            "0279070002",
            normalizedMessage.SampleId);

        Assert.NotEqual(
            default,
            normalizedMessage.ReceivedAtUtc);


        // =====================================================
        // Assert - Results
        // =====================================================

        Assert.Equal(
            3,
            normalizedMessage.Results.Count);


        // =====================================================
        // WBC
        // =====================================================

        LabResult wbc =
            normalizedMessage.Results[0];

        Assert.Equal(
            analyzerId,
            wbc.AnalyzerId);

        Assert.Equal(
            "0279070002",
            wbc.PatientId);

        Assert.Equal(
            "0279070002",
            wbc.SampleId);

        Assert.Equal(
            "WBC",
            wbc.TestCode);

        Assert.Equal(
            "10.7",
            wbc.ResultValue);

        Assert.Equal(
            "10e3/uL^1^1",
            wbc.Units);

        Assert.Equal(
            "4.0-9.0",
            wbc.ReferenceRange);

        Assert.Equal(
            "H",
            wbc.AbnormalFlag);


        // =====================================================
        // RBC
        // =====================================================

        LabResult rbc =
            normalizedMessage.Results[1];

        Assert.Equal(
            "RBC",
            rbc.TestCode);

        Assert.Equal(
            "4.75",
            rbc.ResultValue);


        // =====================================================
        // HGB
        // =====================================================

        LabResult hgb =
            normalizedMessage.Results[2];

        Assert.Equal(
            "HGB",
            hgb.TestCode);

        Assert.Equal(
            "11.8",
            hgb.ResultValue);

        Assert.Equal(
            "g/dL^1^0",
            hgb.Units);

        Assert.Equal(
            "12.0-18.0",
            hgb.ReferenceRange);

        Assert.Equal(
            "L",
            hgb.AbnormalFlag);
    }

    [Fact]
    public void Normalize_AccessHcgMessage_ShouldCreateNormalizedLabResult()
    {
        // =====================================================
        // Arrange
        // =====================================================

        Guid analyzerId =
            Guid.NewGuid();

        string message =
            "H|\\^&|||ACCESS^571576|||||||||20240501123000\r" +
            "P|1||||||||||||||||||||||||\r" +
            "O|1|240501140|^1303^3|^^^HCG5^1||||||||||||||||||||||||F\r" +
            "R|1|^^^HCG5^1|989.87|mIU/mL||||||||||||\r" +
            "L|1";

        var parser =
            new AstmRecordParser();

        AstmMessage parsedMessage =
            parser.Parse(
                message);

        var normalizer =
            new AstmNormalizer();


        // =====================================================
        // Act
        // =====================================================

        IEnumerable<NormalizedLabMessage> normalizedMessages =
            normalizer.Normalize(
                analyzerId,
                parsedMessage);

        NormalizedLabMessage normalizedMessage =
            normalizedMessages.Single();


        // =====================================================
        // Assert - Main Message
        // =====================================================

        Assert.NotEqual(
            Guid.Empty,
            normalizedMessage.Id);

        Assert.Equal(
            analyzerId,
            normalizedMessage.AnalyzerId);

        Assert.Equal(
            "240501140",
            normalizedMessage.SampleId);

        Assert.NotEqual(
            default,
            normalizedMessage.ReceivedAtUtc);


        // =====================================================
        // Assert - Results
        // =====================================================

        Assert.Single(
            normalizedMessage.Results);


        // =====================================================
        // HCG
        // =====================================================

        LabResult hcg =
            normalizedMessage.Results[0];

        Assert.NotEqual(
            Guid.Empty,
            hcg.Id);

        Assert.Equal(
            analyzerId,
            hcg.AnalyzerId);

        Assert.Equal(
            "240501140",
            hcg.SampleId);

        Assert.Equal(
            "HCG5",
            hcg.TestCode);

        Assert.Equal(
            "989.87",
            hcg.ResultValue);

        Assert.Equal(
            "mIU/mL",
            hcg.Units);
    }

    [Fact]
    public void Normalize_AccessFt3Message_ShouldCreateNormalizedLabResult()
    {
        // =====================================================
        // Arrange
        // =====================================================

        Guid analyzerId =
            Guid.NewGuid();

        string message =
            "H|\\^&|||ACCESS^571576|||||||||20240501123000\r" +
            "P|1||||||||||||||||||||||||\r" +
            "O|1|240501141|^1304^3|^^^FT3^1||||||||||||||||||||||||F\r" +
            "R|1|^^^FT3^1|5.42|pg/mL||||||||||||\r" +
            "L|1";

        var parser =
            new AstmRecordParser();

        AstmMessage parsedMessage =
            parser.Parse(
                message);

        var normalizer =
            new AstmNormalizer();


        // =====================================================
        // Act
        // =====================================================

        IEnumerable<NormalizedLabMessage> normalizedMessages =
            normalizer.Normalize(
                analyzerId,
                parsedMessage);

        NormalizedLabMessage normalizedMessage =
            normalizedMessages.Single();


        // =====================================================
        // Assert - Main Message
        // =====================================================

        Assert.NotEqual(
            Guid.Empty,
            normalizedMessage.Id);

        Assert.Equal(
            analyzerId,
            normalizedMessage.AnalyzerId);

        Assert.Equal(
            "240501141",
            normalizedMessage.SampleId);

        Assert.NotEqual(
            default,
            normalizedMessage.ReceivedAtUtc);


        // =====================================================
        // Assert - Results
        // =====================================================

        Assert.Single(
            normalizedMessage.Results);


        // =====================================================
        // FT3
        // =====================================================

        LabResult ft3 =
            normalizedMessage.Results[0];

        Assert.NotEqual(
            Guid.Empty,
            ft3.Id);

        Assert.Equal(
            analyzerId,
            ft3.AnalyzerId);

        Assert.Equal(
            "240501141",
            ft3.SampleId);

        Assert.Equal(
            "FT3",
            ft3.TestCode);

        Assert.Equal(
            "5.42",
            ft3.ResultValue);

        Assert.Equal(
            "pg/mL",
            ft3.Units);
    }
}