using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VieITStore.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSerialTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SerialSanPhams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSerial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SanPhamId = table.Column<int>(type: "int", nullable: false),
                    ChiTietPhieuNhapId = table.Column<int>(type: "int", nullable: true),
                    ChiTietDonHangId = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    NgayBatDauBaoHanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayHetHanBaoHanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerialSanPhams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SerialSanPhams_ChiTietDonHangs_ChiTietDonHangId",
                        column: x => x.ChiTietDonHangId,
                        principalTable: "ChiTietDonHangs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SerialSanPhams_ChiTietPhieuNhaps_ChiTietPhieuNhapId",
                        column: x => x.ChiTietPhieuNhapId,
                        principalTable: "ChiTietPhieuNhaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SerialSanPhams_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SerialSanPhams_ChiTietDonHangId",
                table: "SerialSanPhams",
                column: "ChiTietDonHangId");

            migrationBuilder.CreateIndex(
                name: "IX_SerialSanPhams_ChiTietPhieuNhapId",
                table: "SerialSanPhams",
                column: "ChiTietPhieuNhapId");

            migrationBuilder.CreateIndex(
                name: "IX_SerialSanPhams_MaSerial",
                table: "SerialSanPhams",
                column: "MaSerial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SerialSanPhams_SanPhamId",
                table: "SerialSanPhams",
                column: "SanPhamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SerialSanPhams");
        }
    }
}
