using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BicycleModels",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LifeTimeYears = table.Column<int>(type: "int", nullable: false),
                    ManufacturerCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManufacturerRegion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManufacturerCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufacturerStreet = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BicycleModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bicycles",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsWrittenOff = table.Column<bool>(type: "bit", nullable: false),
                    ModelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    RentalPointCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RentalPointRegion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RentalPointCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RentalPointStreet = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bicycles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bicycles_BicycleModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "BicycleModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bicycles_ModelId",
                table: "Bicycles",
                column: "ModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bicycles");

            migrationBuilder.DropTable(
                name: "BicycleModels");
        }
    }
}
