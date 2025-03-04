using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReviewService.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookId = table.Column<int>(type: "integer", nullable: false),
                    ReviewerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "BookId", "Content", "CreatedAt", "Rating", "ReviewerName" },
                values: new object[,]
                {
                    { 1, 1, "Great book!", new DateTime(2025, 3, 4, 16, 0, 0, 0, DateTimeKind.Utc), 5, "Alice" },
                    { 2, 1, "I didn't like it", new DateTime(2025, 3, 4, 16, 0, 0, 0, DateTimeKind.Utc), 2, "Bob" },
                    { 3, 2, "It was okay", new DateTime(2025, 3, 4, 16, 0, 0, 0, DateTimeKind.Utc), 3, "Charlie" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");
        }
    }
}
