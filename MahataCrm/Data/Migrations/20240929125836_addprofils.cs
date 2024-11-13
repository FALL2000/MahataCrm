using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MahataCrm.Data.Migrations
{
    /// <inheritdoc />
    public partial class addprofils : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfilID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Profils",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isCreateAccount = table.Column<bool>(type: "bit", nullable: false),
                    isUpdateAccount = table.Column<bool>(type: "bit", nullable: false),
                    isDeleteAccount = table.Column<bool>(type: "bit", nullable: false),
                    isBlockAccount = table.Column<bool>(type: "bit", nullable: false),
                    isViewAccount = table.Column<bool>(type: "bit", nullable: false),
                    isCreateServicePlan = table.Column<bool>(type: "bit", nullable: false),
                    isUpdateServicePlan = table.Column<bool>(type: "bit", nullable: false),
                    isDeleteServicePlan = table.Column<bool>(type: "bit", nullable: false),
                    isAssignServicePlan = table.Column<bool>(type: "bit", nullable: false),
                    isViewServicePlan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profils", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProfilID",
                table: "AspNetUsers",
                column: "ProfilID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Profils_ProfilID",
                table: "AspNetUsers",
                column: "ProfilID",
                principalTable: "Profils",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Profils_ProfilID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Profils");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProfilID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilID",
                table: "AspNetUsers");
        }
    }
}
