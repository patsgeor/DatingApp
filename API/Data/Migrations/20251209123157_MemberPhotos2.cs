using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class MemberPhotos2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UrlImage",
                table: "Members",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "DateCreate",
                table: "Members",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "Coutry",
                table: "Members",
                newName: "Country");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Members",
                newName: "UrlImage");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Members",
                newName: "DateCreate");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "Members",
                newName: "Coutry");
        }
    }
}
