using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HTLLBB.Data.Migrations
{
    public partial class ConventionForeignKeys : Migration
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

            migrationBuilder.RenameColumn(
                name: "ForumID",
                table: "Thread",
                newName: "ForumId");

            migrationBuilder.RenameIndex(
                name: "IX_Thread_ForumID",
                table: "Thread",
                newName: "IX_Thread_ForumId");

            migrationBuilder.RenameColumn(
                name: "ThreadID",
                table: "Posts",
                newName: "ThreadId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ThreadID",
                table: "Posts",
                newName: "IX_Posts_ThreadId");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "Forums",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Forums_CategoryID",
                table: "Forums",
                newName: "IX_Forums_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forums_Categories_CategoryId",
                table: "Forums",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Thread_ThreadId",
                table: "Posts",
                column: "ThreadId",
                principalTable: "Thread",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Thread_Forums_ForumId",
                table: "Thread",
                column: "ForumId",
                principalTable: "Forums",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forums_Categories_CategoryId",
                table: "Forums");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Thread_ThreadId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Thread_Forums_ForumId",
                table: "Thread");

            migrationBuilder.RenameColumn(
                name: "ForumId",
                table: "Thread",
                newName: "ForumID");

            migrationBuilder.RenameIndex(
                name: "IX_Thread_ForumId",
                table: "Thread",
                newName: "IX_Thread_ForumID");

            migrationBuilder.RenameColumn(
                name: "ThreadId",
                table: "Posts",
                newName: "ThreadID");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ThreadId",
                table: "Posts",
                newName: "IX_Posts_ThreadID");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Forums",
                newName: "CategoryID");

            migrationBuilder.RenameIndex(
                name: "IX_Forums_CategoryId",
                table: "Forums",
                newName: "IX_Forums_CategoryID");

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
    }
}
