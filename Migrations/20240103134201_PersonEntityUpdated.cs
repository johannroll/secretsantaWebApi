using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecretSantaApi.Migrations
{
    public partial class PersonEntityUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "giverGiftee",
                table: "People",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "giverGiftee",
                table: "People");
        }
    }
}
