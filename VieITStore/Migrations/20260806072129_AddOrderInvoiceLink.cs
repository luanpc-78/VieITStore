using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VieITStore.Migrations;

public partial class AddOrderInvoiceLink : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "ThueVAT",
            table: "HoaDons",
            type: "decimal(5,2)",
            precision: 5,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<string>(
            name: "SoHoaDon",
            table: "HoaDons",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AddColumn<int>(
            name: "DonHangId",
            table: "HoaDons",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_HoaDons_DonHangId",
            table: "HoaDons",
            column: "DonHangId",
            unique: true,
            filter: "[DonHangId] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_HoaDons_SoHoaDon",
            table: "HoaDons",
            column: "SoHoaDon",
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "FK_HoaDons_DonHangs_DonHangId",
            table: "HoaDons",
            column: "DonHangId",
            principalTable: "DonHangs",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_HoaDons_DonHangs_DonHangId", table: "HoaDons");
        migrationBuilder.DropIndex(name: "IX_HoaDons_DonHangId", table: "HoaDons");
        migrationBuilder.DropIndex(name: "IX_HoaDons_SoHoaDon", table: "HoaDons");
        migrationBuilder.DropColumn(name: "DonHangId", table: "HoaDons");

        migrationBuilder.AlterColumn<decimal>(
            name: "ThueVAT",
            table: "HoaDons",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(5,2)",
            oldPrecision: 5,
            oldScale: 2);

        migrationBuilder.AlterColumn<string>(
            name: "SoHoaDon",
            table: "HoaDons",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50);
    }
}
