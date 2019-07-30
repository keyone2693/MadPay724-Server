using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MadPay724.Data.Migrations.MadpayMigrations
{
    public partial class EditeToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpireDate",
                table: "Tokens",
                newName: "Ip");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireTime",
                table: "Tokens",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpireTime",
                table: "Tokens");

            migrationBuilder.RenameColumn(
                name: "Ip",
                table: "Tokens",
                newName: "ExpireDate");
        }
    }
}
