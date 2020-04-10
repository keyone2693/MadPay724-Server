using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MadPay724.Data.Migrations.FinancialMigrations
{
    public partial class updateDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsApprove = table.Column<bool>(nullable: false),
                    IsPardakht = table.Column<bool>(nullable: false),
                    IsReject = table.Column<bool>(nullable: false),
                    Price = table.Column<int>(nullable: false),
                    TextForUser = table.Column<string>(maxLength: 1000, nullable: false),
                    BankName = table.Column<string>(maxLength: 50, nullable: false),
                    OwnerName = table.Column<string>(maxLength: 100, nullable: false),
                    Shaba = table.Column<string>(maxLength: 100, nullable: true),
                    CardNumber = table.Column<string>(maxLength: 20, nullable: false),
                    WalletName = table.Column<string>(maxLength: 20, nullable: false),
                    BankTrackingCode = table.Column<string>(maxLength: 200, nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    BankCardId = table.Column<string>(nullable: false),
                    WalletId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Factors",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(maxLength: 150, nullable: false),
                    RedirectUrl = table.Column<string>(maxLength: 1000, nullable: false),
                    Mobile = table.Column<string>(maxLength: 20, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    FactorNumber = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    ValidCardNumber = table.Column<string>(maxLength: 16, nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Kind = table.Column<short>(nullable: false),
                    Bank = table.Column<short>(nullable: false),
                    GiftCode = table.Column<string>(maxLength: 150, nullable: false),
                    IsGifted = table.Column<bool>(nullable: false),
                    Price = table.Column<int>(nullable: false),
                    EndPrice = table.Column<int>(nullable: false),
                    RefBank = table.Column<string>(maxLength: 500, nullable: false),
                    IsAlreadyVerified = table.Column<bool>(nullable: false),
                    GatewayName = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: false),
                    EnterMoneyWalletId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    GateId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factors", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropTable(
                name: "Factors");
        }
    }
}
