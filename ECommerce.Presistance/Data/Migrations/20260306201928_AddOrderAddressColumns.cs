using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommercePersistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAddressColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Adress_Street",
                table: "Order",
                newName: "Address_Street");

            migrationBuilder.RenameColumn(
                name: "Adress_LastName",
                table: "Order",
                newName: "Address_LastName");

            migrationBuilder.RenameColumn(
                name: "Adress_FirstName",
                table: "Order",
                newName: "Address_FirstName");

            migrationBuilder.RenameColumn(
                name: "Adress_Country",
                table: "Order",
                newName: "Address_Country");

            migrationBuilder.RenameColumn(
                name: "Adress_City",
                table: "Order",
                newName: "Address_City");

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntendId",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentIntendId",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "Address_Street",
                table: "Order",
                newName: "Adress_Street");

            migrationBuilder.RenameColumn(
                name: "Address_LastName",
                table: "Order",
                newName: "Adress_LastName");

            migrationBuilder.RenameColumn(
                name: "Address_FirstName",
                table: "Order",
                newName: "Adress_FirstName");

            migrationBuilder.RenameColumn(
                name: "Address_Country",
                table: "Order",
                newName: "Adress_Country");

            migrationBuilder.RenameColumn(
                name: "Address_City",
                table: "Order",
                newName: "Adress_City");
        }
    }
}
