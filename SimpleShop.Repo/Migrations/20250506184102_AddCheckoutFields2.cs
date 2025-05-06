using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleShop.Repo.Migrations
{
    public partial class AddCheckoutFields2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Baskets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Baskets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryType",
                table: "Baskets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientEmail",
                table: "Baskets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientName",
                table: "Baskets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientPhone",
                table: "Baskets",
                type: "text",
                nullable: false,
                defaultValue: "");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "DeliveryType",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "RecipientEmail",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "RecipientName",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "RecipientPhone",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "Blocked",
                table: "AspNetUsers");
        }
    }
}
