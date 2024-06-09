using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MahataCrm.Data.Migrations
{
    /// <inheritdoc />
    public partial class secondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OperatorId",
                table: "Accounts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_OperatorId",
                table: "Accounts",
                column: "OperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AspNetUsers_OperatorId",
                table: "Accounts",
                column: "OperatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AspNetUsers_OperatorId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_OperatorId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "OperatorId",
                table: "Accounts");
        }
    }
}
