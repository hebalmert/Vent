using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vent.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransfers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transfers_CorporationId_NroTransfer",
                table: "Transfers");

            migrationBuilder.AlterColumn<int>(
                name: "NroTransfer",
                table: "Transfers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromStorageName",
                table: "Transfers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Transfers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToStorageName",
                table: "Transfers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_CorporationId_NroTransfer",
                table: "Transfers",
                columns: new[] { "CorporationId", "NroTransfer" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transfers_CorporationId_NroTransfer",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "FromStorageName",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "ToStorageName",
                table: "Transfers");

            migrationBuilder.AlterColumn<string>(
                name: "NroTransfer",
                table: "Transfers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_CorporationId_NroTransfer",
                table: "Transfers",
                columns: new[] { "CorporationId", "NroTransfer" },
                unique: true,
                filter: "[NroTransfer] IS NOT NULL");
        }
    }
}
