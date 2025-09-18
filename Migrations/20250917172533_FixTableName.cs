using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webcrafters.be_ASP.NET_Core_project.Migrations
{
    /// <inheritdoc />
    public partial class FixTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_contact_messages",
                table: "contact_messages");

            migrationBuilder.RenameTable(
                name: "contact_messages",
                newName: "ContactMessages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactMessages",
                table: "ContactMessages",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactMessages",
                table: "ContactMessages");

            migrationBuilder.RenameTable(
                name: "ContactMessages",
                newName: "contact_messages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_contact_messages",
                table: "contact_messages",
                column: "Id");
        }
    }
}
