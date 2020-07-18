using Microsoft.EntityFrameworkCore.Migrations;

namespace DatingApp.Migrations
{
    public partial class ResolvedFluentApiErrorInLikerConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Users_LikeeId1",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_LikeeId1",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "LikeeId1",
                table: "Likes");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Users_LikerId",
                table: "Likes",
                column: "LikerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Users_LikerId",
                table: "Likes");

            migrationBuilder.AddColumn<int>(
                name: "LikeeId1",
                table: "Likes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Likes_LikeeId1",
                table: "Likes",
                column: "LikeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Users_LikeeId1",
                table: "Likes",
                column: "LikeeId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
