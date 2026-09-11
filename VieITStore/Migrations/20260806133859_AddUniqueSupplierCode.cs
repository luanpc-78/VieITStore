using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VieITStore.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueSupplierCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MaNCC",
                table: "NhaCungCaps",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_NhaCungCaps_MaNCC",
                table: "NhaCungCaps",
                column: "MaNCC",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NhaCungCaps_MaNCC",
                table: "NhaCungCaps");

            migrationBuilder.AlterColumn<string>(
                name: "MaNCC",
                table: "NhaCungCaps",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
