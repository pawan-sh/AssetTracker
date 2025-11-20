using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoicePathToIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoicePath",
                table: "Issues",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoicePath",
                table: "Issues");
        }
    }
}
