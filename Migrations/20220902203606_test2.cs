using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestFinal.Migrations
{
    public partial class test2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Requests_ReciverId",
                table: "Requests",
                column: "ReciverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_ReciverId",
                table: "Requests",
                column: "ReciverId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_ReciverId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_ReciverId",
                table: "Requests");
        }
    }
}
