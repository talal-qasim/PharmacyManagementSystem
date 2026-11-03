using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicineDomainModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenericName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicineCategoryId = table.Column<int>(type: "int", nullable: false),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false),
                    RequiresPrescription = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsControlledOrRestricted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medicines_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Medicines_MedicineCategories_MedicineCategoryId",
                        column: x => x.MedicineCategoryId,
                        principalTable: "MedicineCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    Strength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DosageForm = table.Column<int>(type: "int", nullable: false),
                    PackageSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StockUnit = table.Column<int>(type: "int", nullable: false),
                    UnitsPerPackage = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumStockLevel = table.Column<int>(type: "int", nullable: false),
                    ReorderQuantity = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Medicines_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_ManufacturerId",
                table: "Medicines",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_MedicineCategoryId",
                table: "Medicines",
                column: "MedicineCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_MedicineId",
                table: "ProductVariants",
                column: "MedicineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "Medicines");
        }
    }
}
