using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIssueTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Assets_AssetId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Repairs_Assets_AssetId",
                table: "Repairs");

            migrationBuilder.DropIndex(
                name: "IX_Repairs_AssetId",
                table: "Repairs");

            migrationBuilder.DropIndex(
                name: "IX_Issues_AssetId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "RepairShopName",
                table: "Repairs");

            migrationBuilder.DropColumn(
                name: "ReturnedDate",
                table: "Repairs");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Repairs",
                newName: "RepairNotes");

            migrationBuilder.RenameColumn(
                name: "SentDate",
                table: "Repairs",
                newName: "RepairDate");

            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "Repairs",
                newName: "IssueId");

            migrationBuilder.RenameColumn(
                name: "EmployeeEmail",
                table: "Issues",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Issues",
                newName: "ReportedDate");

            migrationBuilder.AlterColumn<decimal>(
                name: "RepairCost",
                table: "Repairs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetName",
                table: "Issues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReportedBy",
                table: "Issues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_IssueId",
                table: "Repairs",
                column: "IssueId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Repairs_Issues_IssueId",
                table: "Repairs",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "IssueId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repairs_Issues_IssueId",
                table: "Repairs");

            migrationBuilder.DropIndex(
                name: "IX_Repairs_IssueId",
                table: "Repairs");

            migrationBuilder.DropColumn(
                name: "AssetName",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "ReportedBy",
                table: "Issues");

            migrationBuilder.RenameColumn(
                name: "RepairNotes",
                table: "Repairs",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "RepairDate",
                table: "Repairs",
                newName: "SentDate");

            migrationBuilder.RenameColumn(
                name: "IssueId",
                table: "Repairs",
                newName: "AssetId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Issues",
                newName: "EmployeeEmail");

            migrationBuilder.RenameColumn(
                name: "ReportedDate",
                table: "Issues",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<decimal>(
                name: "RepairCost",
                table: "Repairs",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "RepairShopName",
                table: "Repairs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnedDate",
                table: "Repairs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_AssetId",
                table: "Repairs",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_AssetId",
                table: "Issues",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Assets_AssetId",
                table: "Issues",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Repairs_Assets_AssetId",
                table: "Repairs",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
