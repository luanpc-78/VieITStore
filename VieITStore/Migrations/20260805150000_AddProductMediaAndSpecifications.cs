using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VieITStore.Models;

#nullable disable

namespace VieITStore.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260805150000_AddProductMediaAndSpecifications")]
public partial class AddProductMediaAndSpecifications : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name: "HinhAnhBoSung", table: "SanPhams", type: "nvarchar(max)", nullable: true);
        migrationBuilder.AddColumn<string>(name: "ThongSoKyThuat", table: "SanPhams", type: "nvarchar(max)", nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "HinhAnhBoSung", table: "SanPhams");
        migrationBuilder.DropColumn(name: "ThongSoKyThuat", table: "SanPhams");
    }
}
