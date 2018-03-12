using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HTLLBB.Migrations
{
    public partial class Chatbox : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatboxChannels",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Topic = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatboxChannels", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ChatboxMessages",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AuthorId = table.Column<int>(nullable: false),
                    AuthorId1 = table.Column<string>(nullable: false),
                    ChannelId = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatboxMessages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChatboxMessages_AspNetUsers_AuthorId1",
                        column: x => x.AuthorId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatboxMessages_ChatboxChannels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "ChatboxChannels",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatboxMessages_AuthorId1",
                table: "ChatboxMessages",
                column: "AuthorId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatboxMessages_ChannelId",
                table: "ChatboxMessages",
                column: "ChannelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatboxMessages");

            migrationBuilder.DropTable(
                name: "ChatboxChannels");
        }
    }
}
