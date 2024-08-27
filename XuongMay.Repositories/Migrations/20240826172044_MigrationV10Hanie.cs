using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarmentFactory.Repository.Migrations
{
    /// <inheritdoc />
    public partial class MigrationV10Hanie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Orders");
        }
    }
}
