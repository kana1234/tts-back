using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Charts.Shared.Data.Migrations
{
    public partial class Remarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameKz",
                table: "Remarks");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "Remarks");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Remarks",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Remarks",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Remarks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Remarks");

            migrationBuilder.AddColumn<string>(
                name: "NameKz",
                table: "Remarks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "Remarks",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
