using Microsoft.EntityFrameworkCore.Migrations;

namespace ChoreChampion.Data.Migrations
{
    public partial class addedAfterImagePropertyToChore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AfterImage",
                table: "Chore",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AfterImage",
                table: "Chore");
        }
    }
}
