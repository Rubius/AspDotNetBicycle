using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Rename_BicycleBrandsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bicycles_BicycleModels_ModelId",
                table: "Bicycles");

            migrationBuilder.DropTable(
                name: "BicycleModels");

            migrationBuilder.RenameColumn(
                name: "ModelId",
                table: "Bicycles",
                newName: "BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_Bicycles_ModelId",
                table: "Bicycles",
                newName: "IX_Bicycles_BrandId");

            migrationBuilder.CreateTable(
                name: "BicycleBrands",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LifeTimeYears = table.Column<int>(type: "int", nullable: false),
                    ManufacturerCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManufacturerRegion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManufacturerCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufacturerStreet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Class = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BicycleBrands", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Bicycles_BicycleBrands_BrandId",
                table: "Bicycles",
                column: "BrandId",
                principalTable: "BicycleBrands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bicycles_BicycleBrands_BrandId",
                table: "Bicycles");

            migrationBuilder.DropTable(
                name: "BicycleBrands");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "Bicycles",
                newName: "ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Bicycles_BrandId",
                table: "Bicycles",
                newName: "IX_Bicycles_ModelId");

            migrationBuilder.CreateTable(
                name: "BicycleModels",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManufacturerCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufacturerCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManufacturerRegion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManufacturerStreet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Class = table.Column<int>(type: "int", nullable: false),
                    LifeTimeYears = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BicycleModels", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Bicycles_BicycleModels_ModelId",
                table: "Bicycles",
                column: "ModelId",
                principalTable: "BicycleModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
