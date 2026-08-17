using LabAnalyzerConnector.Protocols.ASTM;

namespace LabAnalyzerConnector.Tests;

public sealed class AstmOrderMessageBuilderTests
{
    [Fact]
    public void BuildQueryMessage_ShouldCreateAstmQuery()
    {
        // =====================================================
        // Arrange
        // =====================================================

        var builder =
            new AstmOrderMessageBuilder();

        string barcode =
            "0279070002";


        // =====================================================
        // Act
        // =====================================================

        string message =
            builder.BuildQueryMessage(
                barcode);


        // =====================================================
        // Assert
        // =====================================================

        Assert.Contains(
            "H|\\^&",
            message);

        Assert.Contains(
            "Q|1|0279070002",
            message);

        Assert.Contains(
            "L|1",
            message);
    }


    [Fact]
    public void BuildQueryMessage_ShouldRejectEmptyBarcode()
    {
        // =====================================================
        // Arrange
        // =====================================================

        var builder =
            new AstmOrderMessageBuilder();


        // =====================================================
        // Act & Assert
        // =====================================================

        Assert.Throws<ArgumentException>(
            () =>
                builder.BuildQueryMessage(
                    ""));
    }
}