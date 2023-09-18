using Microsoft.EntityFrameworkCore.Migrations;

namespace IAE.Microservice.Persistence.Migrations
{
    public partial class AddedUserSocialId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "first_name",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "users",
                newName: "name");

            migrationBuilder.AddColumn<long>(
                name: "social_id",
                table: "users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "social_id",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "users",
                newName: "last_name");

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "users",
                type: "text",
                nullable: true);
        }
    }
}
