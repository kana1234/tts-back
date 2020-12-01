using Microsoft.EntityFrameworkCore.Migrations;

namespace Charts.Shared.Data.Migrations
{
    public partial class RemoveUserLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationTask_Users_UserId",
                table: "ApplicationTask");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationTask_UserId",
                table: "ApplicationTask");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ApplicationTask_UserId",
                table: "ApplicationTask",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationTask_Users_UserId",
                table: "ApplicationTask",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
