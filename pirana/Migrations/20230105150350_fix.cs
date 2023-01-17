using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pirana.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortDesc",
                table: "Sliders",
                newName: "SecondaryTitle");

            migrationBuilder.RenameColumn(
                name: "Offer",
                table: "Sliders",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Sliders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Sliders");

            migrationBuilder.RenameColumn(
                name: "SecondaryTitle",
                table: "Sliders",
                newName: "ShortDesc");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Sliders",
                newName: "Offer");
        }
    }
}
