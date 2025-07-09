using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInsertUserandInsertDatecolumnstoTaskstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDate",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "InsertUser",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "InsertUserDetailsId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_InsertUserDetailsId",
                table: "Tasks",
                column: "InsertUserDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_InsertUserDetailsId",
                table: "Tasks",
                column: "InsertUserDetailsId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_InsertUserDetailsId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_InsertUserDetailsId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "InsertUser",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "InsertUserDetailsId",
                table: "Tasks");
        }
    }
}
