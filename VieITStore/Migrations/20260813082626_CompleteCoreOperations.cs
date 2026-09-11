using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VieITStore.Migrations
{
    /// <inheritdoc />
    public partial class CompleteCoreOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhGias_KhachHangs_KhachHangId",
                table: "DanhGias");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGias_SanPhams_SanPhamId",
                table: "DanhGias");

            migrationBuilder.DropIndex(
                name: "IX_DanhGias_KhachHangId",
                table: "DanhGias");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhatTrangThai",
                table: "DonHangs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiCapNhatTrangThaiId",
                table: "DonHangs",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("UPDATE [DanhGias] SET [NoiDung] = N'[Nội dung đánh giá cũ]' WHERE [NoiDung] IS NULL OR LEN(LTRIM(RTRIM([NoiDung]))) < 10;");

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "DanhGias",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhat",
                table: "DanhGias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_NguoiCapNhatTrangThaiId",
                table: "DonHangs",
                column: "NguoiCapNhatTrangThaiId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGias_KhachHangId_SanPhamId",
                table: "DanhGias",
                columns: new[] { "KhachHangId", "SanPhamId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGias_KhachHangs_KhachHangId",
                table: "DanhGias",
                column: "KhachHangId",
                principalTable: "KhachHangs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGias_SanPhams_SanPhamId",
                table: "DanhGias",
                column: "SanPhamId",
                principalTable: "SanPhams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DonHangs_NguoiDungs_NguoiCapNhatTrangThaiId",
                table: "DonHangs",
                column: "NguoiCapNhatTrangThaiId",
                principalTable: "NguoiDungs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhGias_KhachHangs_KhachHangId",
                table: "DanhGias");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGias_SanPhams_SanPhamId",
                table: "DanhGias");

            migrationBuilder.DropForeignKey(
                name: "FK_DonHangs_NguoiDungs_NguoiCapNhatTrangThaiId",
                table: "DonHangs");

            migrationBuilder.DropIndex(
                name: "IX_DonHangs_NguoiCapNhatTrangThaiId",
                table: "DonHangs");

            migrationBuilder.DropIndex(
                name: "IX_DanhGias_KhachHangId_SanPhamId",
                table: "DanhGias");

            migrationBuilder.DropColumn(
                name: "NgayCapNhatTrangThai",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "NguoiCapNhatTrangThaiId",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "NgayCapNhat",
                table: "DanhGias");

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "DanhGias",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.CreateIndex(
                name: "IX_DanhGias_KhachHangId",
                table: "DanhGias",
                column: "KhachHangId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGias_KhachHangs_KhachHangId",
                table: "DanhGias",
                column: "KhachHangId",
                principalTable: "KhachHangs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGias_SanPhams_SanPhamId",
                table: "DanhGias",
                column: "SanPhamId",
                principalTable: "SanPhams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
