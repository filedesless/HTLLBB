using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HTLLBB.Migrations
{
    public partial class ThreadTitleConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Thread",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Thread_Title",
                table: "Thread",
                column: "Title",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Thread_Title",
                table: "Thread");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Thread",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
