using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MigrationPostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class ProductFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFeatures_AspNetUsers_UserId",
                table: "ProductFeatures");

            migrationBuilder.DropIndex(
                name: "IX_ProductFeatures_UserId",
                table: "ProductFeatures");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ProductFeatures");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "ProductFeatures");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProductFeatures");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "ProductFeatures",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "ProductFeatures",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ProductFeatures",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatures_UserId",
                table: "ProductFeatures",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFeatures_AspNetUsers_UserId",
                table: "ProductFeatures",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
