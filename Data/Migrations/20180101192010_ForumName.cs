using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HTLLBB.Data.Migrations
{
    public partial class ForumName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Thread",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Forums",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Categories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Thread");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Forums");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Categories");
        }
    }
}
