using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasarSoftTask3_API.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeographyAuthorities_Users_UsersID1",
                table: "GeographyAuthorities");

            migrationBuilder.DropIndex(
                name: "IX_GeographyAuthorities_UsersID1",
                table: "GeographyAuthorities");

            migrationBuilder.DropColumn(
                name: "UsersID1",
                table: "GeographyAuthorities");

            migrationBuilder.AlterColumn<string>(
                name: "UsersID",
                table: "GeographyAuthorities",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_GeographyAuthorities_UsersID",
                table: "GeographyAuthorities",
                column: "UsersID");

            migrationBuilder.AddForeignKey(
                name: "FK_GeographyAuthorities_Users_UsersID",
                table: "GeographyAuthorities",
                column: "UsersID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeographyAuthorities_Users_UsersID",
                table: "GeographyAuthorities");

            migrationBuilder.DropIndex(
                name: "IX_GeographyAuthorities_UsersID",
                table: "GeographyAuthorities");

            migrationBuilder.AlterColumn<int>(
                name: "UsersID",
                table: "GeographyAuthorities",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "UsersID1",
                table: "GeographyAuthorities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_GeographyAuthorities_UsersID1",
                table: "GeographyAuthorities",
                column: "UsersID1");

            migrationBuilder.AddForeignKey(
                name: "FK_GeographyAuthorities_Users_UsersID1",
                table: "GeographyAuthorities",
                column: "UsersID1",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
