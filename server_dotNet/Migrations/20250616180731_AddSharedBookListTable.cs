using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSharedBookListTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SharedBookLists",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    BookIds = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedBookLists", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharedBookLists");
        }
    }
}
