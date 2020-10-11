using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wave.Migrations
{
    public partial class init1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LatestUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    IsPublic = table.Column<bool>(nullable: false, defaultValue: false),
                    NumberOf = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LatestUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ArtistId = table.Column<string>(nullable: true),
                    Label = table.Column<string>(nullable: true),
                    ReleaseDate = table.Column<DateTime>(nullable: false),
                    ReleaseDatePrecision = table.Column<int>(nullable: false),
                    AlbumType = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LatestUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Albums_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtistImages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LatestUpdate = table.Column<DateTime>(nullable: false),
                    ArtistId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistImages_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistImages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LatestUpdate = table.Column<DateTime>(nullable: false),
                    PlaylistId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaylistImages_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlbumImages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LatestUpdate = table.Column<DateTime>(nullable: false),
                    AlbumId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlbumImages_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AlbumId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Plays = table.Column<int>(nullable: false, defaultValue: 0),
                    IsExplicit = table.Column<bool>(nullable: false, defaultValue: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    DiscNumber = table.Column<int>(nullable: false, defaultValue: 0),
                    NumberOf = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LatestUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tracks_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistElements",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    PlayListId = table.Column<string>(nullable: true),
                    TrackId = table.Column<string>(nullable: true),
                    NumberOf = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LatestUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaylistElements_Playlists_PlayListId",
                        column: x => x.PlayListId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistElements_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TrackFiles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    TrackId = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LatestUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackFiles_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumImages_AlbumId",
                table: "AlbumImages",
                column: "AlbumId",
                unique: true,
                filter: "[AlbumId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_ArtistId",
                table: "Albums",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistImages_ArtistId",
                table: "ArtistImages",
                column: "ArtistId",
                unique: true,
                filter: "[ArtistId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistElements_PlayListId",
                table: "PlaylistElements",
                column: "PlayListId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistElements_TrackId",
                table: "PlaylistElements",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistImages_PlaylistId",
                table: "PlaylistImages",
                column: "PlaylistId",
                unique: true,
                filter: "[PlaylistId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TrackFiles_TrackId",
                table: "TrackFiles",
                column: "TrackId",
                unique: true,
                filter: "[TrackId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_AlbumId",
                table: "Tracks",
                column: "AlbumId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumImages");

            migrationBuilder.DropTable(
                name: "ArtistImages");

            migrationBuilder.DropTable(
                name: "PlaylistElements");

            migrationBuilder.DropTable(
                name: "PlaylistImages");

            migrationBuilder.DropTable(
                name: "TrackFiles");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Artists");
        }
    }
}
