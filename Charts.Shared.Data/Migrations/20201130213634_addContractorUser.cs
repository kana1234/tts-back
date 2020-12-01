using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Charts.Shared.Data.Migrations
{
    public partial class addContractorUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_DicRepairPlace_RepairPlaceId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RepairPlaceId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RepairPlaceId",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "ContractorId",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ContractorId",
                table: "Users",
                column: "ContractorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_DicContractors_ContractorId",
                table: "Users",
                column: "ContractorId",
                principalTable: "DicContractors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_DicContractors_ContractorId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ContractorId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ContractorId",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "RepairPlaceId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RepairPlaceId",
                table: "Users",
                column: "RepairPlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_DicRepairPlace_RepairPlaceId",
                table: "Users",
                column: "RepairPlaceId",
                principalTable: "DicRepairPlace",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
