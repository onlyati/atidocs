using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtiDocs.Migrations
{
    public partial class Version1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    slug = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    folder_id = table.Column<int>(type: "INTEGER", nullable: false),
                    title = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    createat = table.Column<DateTime>(type: "TEXT", nullable: false),
                    changedat = table.Column<DateTime>(type: "TEXT", nullable: false),
                    content = table.Column<string>(type: "TEXT", nullable: true),
                    slug = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.id);
                    table.ForeignKey(
                        name: "FK_Articles_Folders_folder_id",
                        column: x => x.folder_id,
                        principalTable: "Folders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_folder_id",
                table: "Articles",
                column: "folder_id");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_id_slug",
                table: "Articles",
                columns: new[] { "id", "slug" });

            migrationBuilder.CreateIndex(
                name: "IX_Folders_id_slug",
                table: "Folders",
                columns: new[] { "id", "slug" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Folders");
        }
    }
}
