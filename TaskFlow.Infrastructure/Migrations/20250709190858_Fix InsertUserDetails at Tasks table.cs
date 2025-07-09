using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixInsertUserDetailsatTaskstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_InsertUserDetailsId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_InsertUserDetailsId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "InsertUserDetailsId",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_InsertUser",
                table: "Tasks",
                column: "InsertUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_InsertUser",
                table: "Tasks",
                column: "InsertUser",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_InsertUser",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_InsertUser",
                table: "Tasks");

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
    }
}
