using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MigrationPostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class FeatureIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Features_Name_CategoryId",
                table: "Features",
                columns: new[] { "Name", "CategoryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Features_Name_CategoryId",
                table: "Features");
        }
    }
}
