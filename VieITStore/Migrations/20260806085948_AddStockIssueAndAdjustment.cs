using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VieITStore.Migrations;

public partial class AddStockIssueAndAdjustment : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "PhieuDieuChinhTonKhos",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                MaPhieu = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                NguoiThucHienId = table.Column<int>(type: "int", nullable: false),
                LoaiGiaoDich = table.Column<int>(type: "int", nullable: false),
                LyDo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PhieuDieuChinhTonKhos", x => x.Id);
                table.ForeignKey(
                    name: "FK_PhieuDieuChinhTonKhos_NguoiDungs_NguoiThucHienId",
                    column: x => x.NguoiThucHienId,
                    principalTable: "NguoiDungs",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ChiTietDieuChinhTonKhos",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PhieuDieuChinhTonKhoId = table.Column<int>(type: "int", nullable: false),
                SanPhamId = table.Column<int>(type: "int", nullable: false),
                SoLuongTruoc = table.Column<int>(type: "int", nullable: false),
                SoLuongThayDoi = table.Column<int>(type: "int", nullable: false),
                SoLuongSau = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ChiTietDieuChinhTonKhos", x => x.Id);
                table.ForeignKey(
                    name: "FK_ChiTietDieuChinhTonKhos_PhieuDieuChinhTonKhos_PhieuDieuChinhTonKhoId",
                    column: x => x.PhieuDieuChinhTonKhoId,
                    principalTable: "PhieuDieuChinhTonKhos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ChiTietDieuChinhTonKhos_SanPhams_SanPhamId",
                    column: x => x.SanPhamId,
                    principalTable: "SanPhams",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ChiTietDieuChinhTonKhos_PhieuDieuChinhTonKhoId",
            table: "ChiTietDieuChinhTonKhos",
            column: "PhieuDieuChinhTonKhoId");

        migrationBuilder.CreateIndex(
            name: "IX_ChiTietDieuChinhTonKhos_SanPhamId",
            table: "ChiTietDieuChinhTonKhos",
            column: "SanPhamId");

        migrationBuilder.CreateIndex(
            name: "IX_PhieuDieuChinhTonKhos_MaPhieu",
            table: "PhieuDieuChinhTonKhos",
            column: "MaPhieu",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PhieuDieuChinhTonKhos_NguoiThucHienId",
            table: "PhieuDieuChinhTonKhos",
            column: "NguoiThucHienId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ChiTietDieuChinhTonKhos");
        migrationBuilder.DropTable(name: "PhieuDieuChinhTonKhos");
    }
}
