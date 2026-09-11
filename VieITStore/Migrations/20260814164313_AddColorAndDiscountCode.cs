using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VieITStore.Migrations
{
    /// <inheritdoc />
    public partial class AddColorAndDiscountCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MauId",
                table: "SanPhams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaGiamGiaId",
                table: "DanhMucs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MaGiamGias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenKhuyenMai = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanTramGiam = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    GiaGiamToiDa = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GiaToiThieu = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoLuongSuDung = table.Column<int>(type: "int", nullable: true),
                    SoLanDaSuDung = table.Column<int>(type: "int", nullable: false),
                    ApDungToanBo = table.Column<bool>(type: "bit", nullable: false),
                    DanhMucId = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaGiamGias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaGiamGias_DanhMucs_DanhMucId",
                        column: x => x.DanhMucId,
                        principalTable: "DanhMucs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Maus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenMau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maus", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "DanhMucs",
                keyColumn: "Id",
                keyValue: 1,
                column: "MaGiamGiaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanhMucs",
                keyColumn: "Id",
                keyValue: 2,
                column: "MaGiamGiaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanhMucs",
                keyColumn: "Id",
                keyValue: 3,
                column: "MaGiamGiaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DanhMucs",
                keyColumn: "Id",
                keyValue: 4,
                column: "MaGiamGiaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "SanPhams",
                keyColumn: "Id",
                keyValue: 1,
                column: "MauId",
                value: null);

            migrationBuilder.UpdateData(
                table: "SanPhams",
                keyColumn: "Id",
                keyValue: 2,
                column: "MauId",
                value: null);

            migrationBuilder.UpdateData(
                table: "SanPhams",
                keyColumn: "Id",
                keyValue: 3,
                column: "MauId",
                value: null);

            migrationBuilder.UpdateData(
                table: "SanPhams",
                keyColumn: "Id",
                keyValue: 4,
                column: "MauId",
                value: null);

            migrationBuilder.UpdateData(
                table: "SanPhams",
                keyColumn: "Id",
                keyValue: 5,
                column: "MauId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_MauId",
                table: "SanPhams",
                column: "MauId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucs_MaGiamGiaId",
                table: "DanhMucs",
                column: "MaGiamGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_MaGiamGias_DanhMucId",
                table: "MaGiamGias",
                column: "DanhMucId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucs_MaGiamGias_MaGiamGiaId",
                table: "DanhMucs",
                column: "MaGiamGiaId",
                principalTable: "MaGiamGias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_Maus_MauId",
                table: "SanPhams",
                column: "MauId",
                principalTable: "Maus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucs_MaGiamGias_MaGiamGiaId",
                table: "DanhMucs");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_Maus_MauId",
                table: "SanPhams");

            migrationBuilder.DropTable(
                name: "MaGiamGias");

            migrationBuilder.DropTable(
                name: "Maus");

            migrationBuilder.DropIndex(
                name: "IX_SanPhams_MauId",
                table: "SanPhams");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucs_MaGiamGiaId",
                table: "DanhMucs");

            migrationBuilder.DropColumn(
                name: "MauId",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "MaGiamGiaId",
                table: "DanhMucs");
        }
    }
}
