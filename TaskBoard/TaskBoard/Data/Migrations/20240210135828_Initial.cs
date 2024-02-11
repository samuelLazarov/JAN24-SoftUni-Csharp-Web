using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskBoard.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Boards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Board identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, comment: "Board name")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Task identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false, comment: "Task title"),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false, comment: "Task description"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Date of creation"),
                    BoardId = table.Column<int>(type: "int", nullable: true, comment: "Board identifier"),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "Application user identifier")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Board tasks");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "53cb3117-ae21-4485-8900-6b195fe614c2", 0, "55d10319-eb8f-4342-a532-69f856196270", null, false, false, null, null, "TEST@SOFTUNI.BG", "AQAAAAEAACcQAAAAEAeqIjaPtT3grGbbKtzjwM8CMTOxNrrJeyoSrs541HFtTIbv+jhmk0c1qAl8VnovUQ==", null, false, "cabdcaf2-c26d-4f09-ae3e-660f40279773", false, "Test" });

            migrationBuilder.InsertData(
                table: "Boards",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Open" },
                    { 2, "In Progress" },
                    { 3, "Done" }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "BoardId", "CreatedOn", "Description", "OwnerId", "Title" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2023, 7, 25, 15, 58, 28, 590, DateTimeKind.Local).AddTicks(7458), "Implement better styling for all public pages", "53cb3117-ae21-4485-8900-6b195fe614c2", "Improve CSS styles" },
                    { 2, 1, new DateTime(2023, 9, 10, 15, 58, 28, 590, DateTimeKind.Local).AddTicks(7491), "Create Android client app for the TaskBoard RESTful API", "53cb3117-ae21-4485-8900-6b195fe614c2", "Android Client App" },
                    { 3, 2, new DateTime(2024, 1, 10, 15, 58, 28, 590, DateTimeKind.Local).AddTicks(7495), "Create Windows Forms desktop app client for the TaskBoard RESTful API", "53cb3117-ae21-4485-8900-6b195fe614c2", "Desktop Client App" },
                    { 4, 3, new DateTime(2023, 2, 10, 15, 58, 28, 590, DateTimeKind.Local).AddTicks(7498), "Implement [Create Task] page for adding new tasks", "53cb3117-ae21-4485-8900-6b195fe614c2", "Create Tasks" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_BoardId",
                table: "Tasks",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OwnerId",
                table: "Tasks",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Boards");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "53cb3117-ae21-4485-8900-6b195fe614c2");
        }
    }
}
