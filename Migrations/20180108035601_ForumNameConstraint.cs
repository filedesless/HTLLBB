using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HTLLBB.Migrations
{
    public partial class ForumNameConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Forums",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Forums_Name",
                table: "Forums",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Forums_Name",
                table: "Forums");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Forums",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
