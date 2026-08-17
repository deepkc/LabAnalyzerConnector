using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabAnalyzerConnector.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalyzerMappingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

           

            migrationBuilder.CreateTable(
                name: "TestCodeMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AnalyzerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AnalyzerTestCode = table.Column<string>(type: "TEXT", nullable: false),
                    AnalyzerTestName = table.Column<string>(type: "TEXT", nullable: true),
                    StandardTestCode = table.Column<string>(type: "TEXT", nullable: false),
                    StandardTestName = table.Column<string>(type: "TEXT", nullable: true),
                    ExpectedUnit = table.Column<string>(type: "TEXT", nullable: true),
                    StandardUnit = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCodeMappings", x => x.Id);
                });


            migrationBuilder.CreateIndex(
                name: "IX_TestCodeMappings_AnalyzerId",
                table: "TestCodeMappings",
                column: "AnalyzerId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCodeMappings_AnalyzerId_AnalyzerTestCode",
                table: "TestCodeMappings",
                columns: new[] { "AnalyzerId", "AnalyzerTestCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropTable(
                name: "AnalyzerMappingProfiles");

           

            migrationBuilder.DropTable(
                name: "TestCodeMappings");
        }
    }
}
