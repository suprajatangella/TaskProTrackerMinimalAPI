using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskProTracker.MinimalAPI.Migrations
{
    /// <inheritdoc />
    public partial class FinalFixDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Tasks_TaskItemId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_TaskItemId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TaskItemId",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId",
                table: "Tasks",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_ProjectId",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "TaskItemId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_TaskItemId",
                table: "Projects",
                column: "TaskItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Tasks_TaskItemId",
                table: "Projects",
                column: "TaskItemId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
