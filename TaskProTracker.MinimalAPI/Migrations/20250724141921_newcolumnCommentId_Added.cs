using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskProTracker.MinimalAPI.Migrations
{
    /// <inheritdoc />
    public partial class newcolumnCommentId_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<int>(
            //    name: "CommentId",
            //    table: "Tasks",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<int>(
            //    name: "CommentId",
            //    table: "Tasks",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Tasks_CommentId",
            //    table: "Tasks",
            //    column: "CommentId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Tasks_Comments_CommentId",
            //    table: "Tasks",
            //    column: "CommentId",
            //    principalTable: "Comments",
            //    principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Comments_CommentId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CommentId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Tasks");
        }
    }
}
