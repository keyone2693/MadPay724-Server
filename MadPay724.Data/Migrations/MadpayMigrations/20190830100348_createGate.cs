using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MadPay724.Data.Migrations.MadpayMigrations
{
    public partial class createGate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gate",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Ip = table.Column<bool>(nullable: false),
                    WebsiteName = table.Column<string>(maxLength: 100, nullable: false),
                    WebsiteUrl = table.Column<string>(maxLength: 500, nullable: false),
                    PhonrNumber = table.Column<string>(maxLength: 50, nullable: false),
                    Text = table.Column<string>(maxLength: 1000, nullable: false),
                    Grouping = table.Column<string>(maxLength: 50, nullable: false),
                    IconUrl = table.Column<string>(maxLength: 1000, nullable: false),
                    WalletId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gate_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gate_WalletId",
                table: "Gate",
                column: "WalletId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gate");
        }
    }
}
