using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MadPay724.Data.Migrations.MadpayMigrations
{
    public partial class addVC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VerificationCodes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(maxLength: 5, nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    RemoveDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationCodes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerificationCodes");
        }
    }
}
