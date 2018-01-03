using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HTLLBB.Data.Migrations
{
    public partial class CascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forums_Categories_CategoryID",
                table: "Forums");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Thread_ThreadID",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Thread_Forums_ForumID",
                table: "Thread");

            migrationBuilder.AlterColumn<int>(
                name: "ForumID",
                table: "Thread",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ThreadID",
                table: "Posts",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryID",
                table: "Forums",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Forums_Categories_CategoryID",
                table: "Forums",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Thread_ThreadID",
                table: "Posts",
                column: "ThreadID",
                principalTable: "Thread",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Thread_Forums_ForumID",
                table: "Thread",
                column: "ForumID",
                principalTable: "Forums",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forums_Categories_CategoryID",
                table: "Forums");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Thread_ThreadID",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Thread_Forums_ForumID",
                table: "Thread");

            migrationBuilder.AlterColumn<int>(
                name: "ForumID",
                table: "Thread",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ThreadID",
                table: "Posts",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CategoryID",
                table: "Forums",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Forums_Categories_CategoryID",
                table: "Forums",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Thread_ThreadID",
                table: "Posts",
                column: "ThreadID",
                principalTable: "Thread",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Thread_Forums_ForumID",
                table: "Thread",
                column: "ForumID",
                principalTable: "Forums",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
