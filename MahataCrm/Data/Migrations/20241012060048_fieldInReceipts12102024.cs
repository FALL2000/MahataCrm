using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MahataCrm.Data.Migrations
{
    /// <inheritdoc />
    public partial class fieldInReceipts12102024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkVerification",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalTax",
                table: "Receipts",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkVerification",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "TotalTax",
                table: "Receipts");
        }
    }
}
