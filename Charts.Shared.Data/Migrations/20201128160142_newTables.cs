using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Charts.Shared.Data.Migrations
{
    public partial class newTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RepairPlaceId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RepairType",
                table: "Applications",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "WithReplacement",
                table: "Applications",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ApplicationHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ApplicationId = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<Guid>(nullable: true),
                    AppointmentDate = table.Column<DateTime>(nullable: true),
                    PlanEndDate = table.Column<DateTime>(nullable: true),
                    FactEndDate = table.Column<DateTime>(nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationHistory_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationHistory_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationHistory_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ApplicationId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<Guid>(nullable: true),
                    AppointmentDate = table.Column<DateTime>(nullable: true),
                    PlanEndDate = table.Column<DateTime>(nullable: true),
                    FactEndDate = table.Column<DateTime>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationTask_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationTask_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationTask_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocInformation",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    IsOriginal = table.Column<bool>(nullable: false),
                    DocumentCode = table.Column<string>(nullable: true),
                    PageCount = table.Column<int>(nullable: false),
                    PageInterval = table.Column<string>(maxLength: 50, nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Page = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocInformation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TTSFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Size = table.Column<int>(nullable: false),
                    FileTypeId = table.Column<Guid>(nullable: true),
                    FirstDocTypeId = table.Column<Guid>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    CreatorUserId = table.Column<Guid>(nullable: true),
                    DeleteDateTime = table.Column<DateTime>(nullable: true),
                    DeletedUserId = table.Column<Guid>(nullable: true),
                    IsActual = table.Column<bool>(nullable: false),
                    DocInformationId = table.Column<Guid>(nullable: true),
                    ApplicationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTSFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TTSFile_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TTSFile_Users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TTSFile_Users_DeletedUserId",
                        column: x => x.DeletedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TTSFile_DocInformation_DocInformationId",
                        column: x => x.DocInformationId,
                        principalTable: "DocInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RepairPlaceId",
                table: "Users",
                column: "RepairPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationHistory_ApplicationId",
                table: "ApplicationHistory",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationHistory_RoleId",
                table: "ApplicationHistory",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationHistory_UserId",
                table: "ApplicationHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationTask_ApplicationId",
                table: "ApplicationTask",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationTask_RoleId",
                table: "ApplicationTask",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationTask_UserId",
                table: "ApplicationTask",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TTSFile_ApplicationId",
                table: "TTSFile",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TTSFile_CreatorUserId",
                table: "TTSFile",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TTSFile_DeletedUserId",
                table: "TTSFile",
                column: "DeletedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TTSFile_DocInformationId",
                table: "TTSFile",
                column: "DocInformationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_DicRepairPlace_RepairPlaceId",
                table: "Users",
                column: "RepairPlaceId",
                principalTable: "DicRepairPlace",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_DicRepairPlace_RepairPlaceId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ApplicationHistory");

            migrationBuilder.DropTable(
                name: "ApplicationTask");

            migrationBuilder.DropTable(
                name: "TTSFile");

            migrationBuilder.DropTable(
                name: "DocInformation");

            migrationBuilder.DropIndex(
                name: "IX_Users_RepairPlaceId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RepairPlaceId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RepairType",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "WithReplacement",
                table: "Applications");
        }
    }
}
