using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VieITStore.Migrations;

public partial class AddPointOfSaleChannel : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "MaDonHang",
            table: "DonHangs",
            type: "nvarchar(40)",
            maxLength: 40,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AddColumn<string>(
            name: "KenhBan",
            table: "DonHangs",
            type: "nvarchar(30)",
            maxLength: 30,
            nullable: false,
            defaultValue: "Trực tuyến");

        migrationBuilder.CreateIndex(
            name: "IX_DonHangs_MaDonHang",
            table: "DonHangs",
            column: "MaDonHang",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_DonHangs_MaDonHang", table: "DonHangs");
        migrationBuilder.DropColumn(name: "KenhBan", table: "DonHangs");
        migrationBuilder.AlterColumn<string>(
            name: "MaDonHang",
            table: "DonHangs",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(40)",
            oldMaxLength: 40);
    }
}
