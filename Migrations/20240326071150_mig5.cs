using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace BasarSoftTask3_API.Migrations
{
    /// <inheritdoc />
    public partial class mig5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Geometry",
                table: "GeographyAuthorities");

            migrationBuilder.RenameColumn(
                name: "GeometryID",
                table: "GeographyAuthorities",
                newName: "LocationID");

            migrationBuilder.AddColumn<int>(
                name: "LocAndUsersID",
                table: "GeographyAuthorities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GeographyAuthorities_LocAndUsersID",
                table: "GeographyAuthorities",
                column: "LocAndUsersID");

            migrationBuilder.AddForeignKey(
                name: "FK_GeographyAuthorities_LocsAndUsers_LocAndUsersID",
                table: "GeographyAuthorities",
                column: "LocAndUsersID",
                principalTable: "LocsAndUsers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeographyAuthorities_LocsAndUsers_LocAndUsersID",
                table: "GeographyAuthorities");

            migrationBuilder.DropIndex(
                name: "IX_GeographyAuthorities_LocAndUsersID",
                table: "GeographyAuthorities");

            migrationBuilder.DropColumn(
                name: "LocAndUsersID",
                table: "GeographyAuthorities");

            migrationBuilder.RenameColumn(
                name: "LocationID",
                table: "GeographyAuthorities",
                newName: "GeometryID");

            migrationBuilder.AddColumn<Geometry>(
                name: "Geometry",
                table: "GeographyAuthorities",
                type: "geometry",
                nullable: false);
        }
    }
}
