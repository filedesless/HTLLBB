using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HTLLBB.Migrations
{
    public partial class FixedCircularDependencies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Thread_Posts_FirstPostId",
                table: "Thread");

            migrationBuilder.DropIndex(
                name: "IX_Thread_FirstPostId",
                table: "Thread");

            migrationBuilder.DropColumn(
                name: "FirstPostId",
                table: "Thread");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FirstPostId",
                table: "Thread",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Thread_FirstPostId",
                table: "Thread",
                column: "FirstPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Thread_Posts_FirstPostId",
                table: "Thread",
                column: "FirstPostId",
                principalTable: "Posts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
