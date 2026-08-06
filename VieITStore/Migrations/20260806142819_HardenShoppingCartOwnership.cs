using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VieITStore.Migrations
{
    /// <inheritdoc />
    public partial class HardenShoppingCartOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_GioHangs_NguoiDungs_NguoiDungId", table: "GioHangs");
            migrationBuilder.DropForeignKey(name: "FK_GioHangs_SanPhams_SanPhamId", table: "GioHangs");
            migrationBuilder.DropIndex(name: "IX_GioHangs_NguoiDungId", table: "GioHangs");

            migrationBuilder.Sql("""
                UPDATE [GioHangs] SET [SessionId] = NULL WHERE [NguoiDungId] IS NOT NULL;
                DELETE FROM [GioHangs] WHERE [NguoiDungId] IS NULL AND ([SessionId] IS NULL OR LTRIM(RTRIM([SessionId])) = '');
                UPDATE [GioHangs] SET [SessionId] = LEFT([SessionId], 100) WHERE LEN([SessionId]) > 100;
                UPDATE [GioHangs] SET [SoLuong] = 1 WHERE [SoLuong] <= 0;

                WITH [Ranked] AS (
                    SELECT [Id], SUM([SoLuong]) OVER (PARTITION BY [NguoiDungId], [SanPhamId]) AS [Tong],
                           ROW_NUMBER() OVER (PARTITION BY [NguoiDungId], [SanPhamId] ORDER BY [Id]) AS [ThuTu]
                    FROM [GioHangs] WHERE [NguoiDungId] IS NOT NULL
                )
                UPDATE [g] SET [SoLuong] = [r].[Tong] FROM [GioHangs] [g]
                INNER JOIN [Ranked] [r] ON [g].[Id] = [r].[Id] WHERE [r].[ThuTu] = 1;
                WITH [Ranked] AS (
                    SELECT [Id], ROW_NUMBER() OVER (PARTITION BY [NguoiDungId], [SanPhamId] ORDER BY [Id]) AS [ThuTu]
                    FROM [GioHangs] WHERE [NguoiDungId] IS NOT NULL
                )
                DELETE [g] FROM [GioHangs] [g] INNER JOIN [Ranked] [r] ON [g].[Id] = [r].[Id] WHERE [r].[ThuTu] > 1;

                WITH [Ranked] AS (
                    SELECT [Id], SUM([SoLuong]) OVER (PARTITION BY [SessionId], [SanPhamId]) AS [Tong],
                           ROW_NUMBER() OVER (PARTITION BY [SessionId], [SanPhamId] ORDER BY [Id]) AS [ThuTu]
                    FROM [GioHangs] WHERE [SessionId] IS NOT NULL
                )
                UPDATE [g] SET [SoLuong] = [r].[Tong] FROM [GioHangs] [g]
                INNER JOIN [Ranked] [r] ON [g].[Id] = [r].[Id] WHERE [r].[ThuTu] = 1;
                WITH [Ranked] AS (
                    SELECT [Id], ROW_NUMBER() OVER (PARTITION BY [SessionId], [SanPhamId] ORDER BY [Id]) AS [ThuTu]
                    FROM [GioHangs] WHERE [SessionId] IS NOT NULL
                )
                DELETE [g] FROM [GioHangs] [g] INNER JOIN [Ranked] [r] ON [g].[Id] = [r].[Id] WHERE [r].[ThuTu] > 1;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId", table: "GioHangs", type: "nvarchar(100)", maxLength: 100,
                nullable: true, oldClrType: typeof(string), oldType: "nvarchar(max)", oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GioHangs_NguoiDungId_SanPhamId", table: "GioHangs",
                columns: new[] { "NguoiDungId", "SanPhamId" }, unique: true,
                filter: "[NguoiDungId] IS NOT NULL");
            migrationBuilder.CreateIndex(
                name: "IX_GioHangs_SessionId_SanPhamId", table: "GioHangs",
                columns: new[] { "SessionId", "SanPhamId" }, unique: true,
                filter: "[SessionId] IS NOT NULL");
            migrationBuilder.AddCheckConstraint(
                name: "CK_GioHangs_Owner", table: "GioHangs",
                sql: "([NguoiDungId] IS NOT NULL AND [SessionId] IS NULL) OR ([NguoiDungId] IS NULL AND [SessionId] IS NOT NULL)");
            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_NguoiDungs_NguoiDungId", table: "GioHangs", column: "NguoiDungId",
                principalTable: "NguoiDungs", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_SanPhams_SanPhamId", table: "GioHangs", column: "SanPhamId",
                principalTable: "SanPhams", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_GioHangs_NguoiDungs_NguoiDungId", table: "GioHangs");
            migrationBuilder.DropForeignKey(name: "FK_GioHangs_SanPhams_SanPhamId", table: "GioHangs");
            migrationBuilder.DropIndex(name: "IX_GioHangs_NguoiDungId_SanPhamId", table: "GioHangs");
            migrationBuilder.DropIndex(name: "IX_GioHangs_SessionId_SanPhamId", table: "GioHangs");
            migrationBuilder.DropCheckConstraint(name: "CK_GioHangs_Owner", table: "GioHangs");
            migrationBuilder.AlterColumn<string>(
                name: "SessionId", table: "GioHangs", type: "nvarchar(max)", nullable: true,
                oldClrType: typeof(string), oldType: "nvarchar(100)", oldMaxLength: 100, oldNullable: true);
            migrationBuilder.CreateIndex(name: "IX_GioHangs_NguoiDungId", table: "GioHangs", column: "NguoiDungId");
            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_NguoiDungs_NguoiDungId", table: "GioHangs", column: "NguoiDungId",
                principalTable: "NguoiDungs", principalColumn: "Id");
            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_SanPhams_SanPhamId", table: "GioHangs", column: "SanPhamId",
                principalTable: "SanPhams", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
        }
    }
}
