using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MahataCrm.Data.Migrations
{
    /// <inheritdoc />
    public partial class addopmatchMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperatorMatchAccount_AspNetUsers_OperatorID",
                table: "OperatorMatchAccount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperatorMatchAccount",
                table: "OperatorMatchAccount");

            migrationBuilder.RenameTable(
                name: "OperatorMatchAccount",
                newName: "OperatorMatchs");

            migrationBuilder.RenameIndex(
                name: "IX_OperatorMatchAccount_OperatorID",
                table: "OperatorMatchs",
                newName: "IX_OperatorMatchs_OperatorID");

            migrationBuilder.RenameIndex(
                name: "IX_OperatorMatchAccount_BussinessId",
                table: "OperatorMatchs",
                newName: "IX_OperatorMatchs_BussinessId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperatorMatchs",
                table: "OperatorMatchs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperatorMatchs_AspNetUsers_OperatorID",
                table: "OperatorMatchs",
                column: "OperatorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperatorMatchs_AspNetUsers_OperatorID",
                table: "OperatorMatchs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperatorMatchs",
                table: "OperatorMatchs");

            migrationBuilder.RenameTable(
                name: "OperatorMatchs",
                newName: "OperatorMatchAccount");

            migrationBuilder.RenameIndex(
                name: "IX_OperatorMatchs_OperatorID",
                table: "OperatorMatchAccount",
                newName: "IX_OperatorMatchAccount_OperatorID");

            migrationBuilder.RenameIndex(
                name: "IX_OperatorMatchs_BussinessId",
                table: "OperatorMatchAccount",
                newName: "IX_OperatorMatchAccount_BussinessId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperatorMatchAccount",
                table: "OperatorMatchAccount",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperatorMatchAccount_AspNetUsers_OperatorID",
                table: "OperatorMatchAccount",
                column: "OperatorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
