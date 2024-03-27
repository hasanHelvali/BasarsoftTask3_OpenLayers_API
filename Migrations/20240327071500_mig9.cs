using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasarSoftTask3_API.Migrations
{
    /// <inheritdoc />
    public partial class mig9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeographyAuthorities_LocsAndUsers_LocAndUsersID",
                table: "GeographyAuthorities");

            migrationBuilder.DropForeignKey(
                name: "FK_GeographyAuthorities_Users_UsersID",
                table: "GeographyAuthorities");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_GeographyAuthorities_LocAndUsersID",
                table: "GeographyAuthorities");

            migrationBuilder.DropIndex(
                name: "IX_GeographyAuthorities_UsersID",
                table: "GeographyAuthorities");

            migrationBuilder.DropColumn(
                name: "LocAndUsersID",
                table: "GeographyAuthorities");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "LocsAndUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "LocsAndUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UsersID",
                table: "GeographyAuthorities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "LocationID",
                table: "GeographyAuthorities",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "LocsAndUsers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "LocsAndUsers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UsersID",
                table: "GeographyAuthorities",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LocationID",
                table: "GeographyAuthorities",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocAndUsersID",
                table: "GeographyAuthorities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<List<string>>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeographyAuthorities_LocAndUsersID",
                table: "GeographyAuthorities",
                column: "LocAndUsersID");

            migrationBuilder.CreateIndex(
                name: "IX_GeographyAuthorities_UsersID",
                table: "GeographyAuthorities",
                column: "UsersID");

            migrationBuilder.AddForeignKey(
                name: "FK_GeographyAuthorities_LocsAndUsers_LocAndUsersID",
                table: "GeographyAuthorities",
                column: "LocAndUsersID",
                principalTable: "LocsAndUsers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GeographyAuthorities_Users_UsersID",
                table: "GeographyAuthorities",
                column: "UsersID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
