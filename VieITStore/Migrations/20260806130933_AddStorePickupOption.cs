using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VieITStore.Migrations
{
    /// <inheritdoc />
    public partial class AddStorePickupOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HinhThucNhanHang",
                table: "DonHangs",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhThucNhanHang",
                table: "DonHangs");
        }
    }
}
