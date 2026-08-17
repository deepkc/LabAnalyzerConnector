using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabAnalyzerSimulator.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Barcode = table.Column<string>(type: "TEXT", nullable: false),
                    TestCode = table.Column<string>(type: "TEXT", nullable: false),
                    TestName = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Barcode = table.Column<string>(type: "TEXT", nullable: false),
                    PatientId = table.Column<string>(type: "TEXT", nullable: false),
                    PatientName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Barcode = table.Column<string>(type: "TEXT", nullable: false),
                    TestCode = table.Column<string>(type: "TEXT", nullable: false),
                    Result = table.Column<string>(type: "TEXT", nullable: false),
                    Units = table.Column<string>(type: "TEXT", nullable: false),
                    ReferenceRange = table.Column<string>(type: "TEXT", nullable: false),
                    Flag = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Barcode",
                table: "Orders",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Barcode",
                table: "Patients",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Results_Barcode",
                table: "Results",
                column: "Barcode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Results");
        }
    }
}
