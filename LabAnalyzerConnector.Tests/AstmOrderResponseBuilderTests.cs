using LabAnalyzerConnector.Core.Models;
using LabAnalyzerConnector.Protocols.ASTM;

namespace LabAnalyzerConnector.Tests;

public sealed class AstmOrderResponseBuilderTests
{
    [Fact]
    public void Build_ShouldCreateAstmOrderResponse()
    {
        // =====================================================
        // Arrange
        // =====================================================

        var order = new LabOrder
        {
            OrderId = "ORD-001",

            PatientId = "PAT-001",

            PatientName = "Test Patient",

            SpecimenId = "SPEC-001",

            Barcode = "0279070002",

            OrderedTests =
                new List<string>
                {
                    "CBC",
                    "HGB",
                    "WBC"
                },

            Priority = "Routine",

            Status = "Pending"
        };


        var builder =
            new AstmOrderResponseBuilder();


        // =====================================================
        // Act
        // =====================================================

        string response =
            builder.Build(
                order);


        // =====================================================
        // Assert
        // =====================================================

        Assert.False(
            string.IsNullOrWhiteSpace(
                response));


        Assert.Contains(
            "0279070002",
            response);


        Assert.Contains(
            "CBC",
            response);


        Assert.Contains(
            "HGB",
            response);


        Assert.Contains(
            "WBC",
            response);


        Assert.Contains(
            "H|",
            response);


        Assert.Contains(
            "P|",
            response);


        Assert.Contains(
            "O|",
            response);


        Assert.Contains(
            "L|",
            response);
    }
}