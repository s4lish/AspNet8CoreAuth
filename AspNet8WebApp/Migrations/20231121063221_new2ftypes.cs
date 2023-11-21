using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNet8WebApp.Migrations
{
    /// <inheritdoc />
    public partial class new2ftypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Twofactortypes",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Twofactortypes",
                table: "AspNetUsers");
        }
    }
}
