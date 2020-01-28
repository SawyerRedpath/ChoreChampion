using Microsoft.EntityFrameworkCore.Migrations;

namespace ChoreChampion.Data.Migrations
{
    public partial class addedUserIdToChore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Chore",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chore_UserId",
                table: "Chore",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chore_AspNetUsers_UserId",
                table: "Chore",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chore_AspNetUsers_UserId",
                table: "Chore");

            migrationBuilder.DropIndex(
                name: "IX_Chore_UserId",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Chore");
        }
    }
}
