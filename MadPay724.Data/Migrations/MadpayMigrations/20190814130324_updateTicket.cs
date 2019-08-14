using Microsoft.EntityFrameworkCore.Migrations;

namespace MadPay724.Data.Migrations.MadpayMigrations
{
    public partial class updateTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Tickets",
                newName: "Closed");

            migrationBuilder.AddColumn<short>(
                name: "Department",
                table: "Tickets",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "Closed",
                table: "Tickets",
                newName: "Status");
        }
    }
}
