using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MahataCrm.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOpMatchAccMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperatorMatchAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperatorID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BussinessId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatorMatchAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperatorMatchAccount_AspNetUsers_OperatorID",
                        column: x => x.OperatorID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperatorMatchAccount_BussinessId",
                table: "OperatorMatchAccount",
                column: "BussinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperatorMatchAccount_OperatorID",
                table: "OperatorMatchAccount",
                column: "OperatorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperatorMatchAccount");
        }
    }
}
