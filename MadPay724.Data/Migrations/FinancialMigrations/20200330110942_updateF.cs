using Microsoft.EntityFrameworkCore.Migrations;

namespace MadPay724.Data.Migrations.FinancialMigrations
{
    public partial class updateF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GatewayName",
                table: "Factors",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAlreadyVerified",
                table: "Factors",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Factors",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GatewayName",
                table: "Factors");

            migrationBuilder.DropColumn(
                name: "IsAlreadyVerified",
                table: "Factors");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Factors");
        }
    }
}
