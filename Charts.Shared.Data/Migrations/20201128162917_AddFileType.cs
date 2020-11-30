using Microsoft.EntityFrameworkCore.Migrations;

namespace Charts.Shared.Data.Migrations
{
    public partial class AddFileType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Page",
                table: "DocInformation");

            migrationBuilder.AddColumn<int>(
                name: "FileType",
                table: "TTSFile",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FileType",
                table: "DocInformation",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileType",
                table: "TTSFile");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "DocInformation");

            migrationBuilder.AddColumn<int>(
                name: "Page",
                table: "DocInformation",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
