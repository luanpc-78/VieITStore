using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VieITStore.Migrations
{
    /// <inheritdoc />
    public partial class AddProductBarcodeLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [SanPhams] SET [MaSanPham] = LTRIM(RTRIM([MaSanPham]));");

            migrationBuilder.AlterColumn<string>(
                name: "MaSanPham",
                table: "SanPhams",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "MaVach",
                table: "SanPhams",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_MaSanPham",
                table: "SanPhams",
                column: "MaSanPham",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_MaVach",
                table: "SanPhams",
                column: "MaVach",
                unique: true,
                filter: "[MaVach] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_SanPhams_MaSanPham", table: "SanPhams");
            migrationBuilder.DropIndex(name: "IX_SanPhams_MaVach", table: "SanPhams");
            migrationBuilder.DropColumn(name: "MaVach", table: "SanPhams");

            migrationBuilder.AlterColumn<string>(
                name: "MaSanPham",
                table: "SanPhams",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
