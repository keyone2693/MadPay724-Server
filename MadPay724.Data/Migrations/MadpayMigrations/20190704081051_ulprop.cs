using Microsoft.EntityFrameworkCore.Migrations;

namespace MadPay724.Data.Migrations.MadpayMigrations
{
    public partial class ulprop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UploadLocal",
                table: "Settings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadLocal",
                table: "Settings");
        }
    }
}
