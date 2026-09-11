using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VieITStore.Migrations
{
    /// <inheritdoc />
    public partial class UseEmailAsLoginIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [NguoiDungs]
                SET [Email] = LOWER(LTRIM(RTRIM([Email])))
                WHERE [Email] IS NOT NULL AND LTRIM(RTRIM([Email])) <> N'';

                UPDATE [NguoiDungs]
                SET [Email] = CONCAT(N'legacy-', [Id], N'@vieitstore.local')
                WHERE [Email] IS NULL OR LTRIM(RTRIM([Email])) = N'';

                ;WITH DuplicateEmails AS
                (
                    SELECT [Id], [Email], ROW_NUMBER() OVER (PARTITION BY [Email] ORDER BY [Id]) AS [RowNumber]
                    FROM [NguoiDungs]
                )
                UPDATE n
                SET n.[Email] = CONCAT(N'legacy-', n.[Id], N'@vieitstore.local')
                FROM [NguoiDungs] n
                INNER JOIN DuplicateEmails d ON d.[Id] = n.[Id]
                WHERE d.[RowNumber] > 1;

                UPDATE [NguoiDungs]
                SET [TenDangNhap] = [Email]
                WHERE [Id] IN (1, 2, 3)
                   OR [TenDangNhap] IS NULL
                   OR LTRIM(RTRIM([TenDangNhap])) = N'';
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "NguoiDungs",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungs_Email",
                table: "NguoiDungs",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NguoiDungs_Email",
                table: "NguoiDungs");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "NguoiDungs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);
        }
    }
}
