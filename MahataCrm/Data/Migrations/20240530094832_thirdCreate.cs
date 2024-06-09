using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MahataCrm.Data.Migrations
{
    /// <inheritdoc />
    public partial class thirdCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AspNetUsers_OperatorId",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "OperatorId",
                table: "Accounts",
                newName: "OperatorID");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_OperatorId",
                table: "Accounts",
                newName: "IX_Accounts_OperatorID");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OperatorID",
                table: "Accounts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AspNetUsers_OperatorID",
                table: "Accounts",
                column: "OperatorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AspNetUsers_OperatorID",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "OperatorID",
                table: "Accounts",
                newName: "OperatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_OperatorID",
                table: "Accounts",
                newName: "IX_Accounts_OperatorId");

            migrationBuilder.AlterColumn<string>(
                name: "OperatorId",
                table: "Accounts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AspNetUsers_OperatorId",
                table: "Accounts",
                column: "OperatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
