using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MadPay724.Data.Migrations.MadpayMigrations
{
    public partial class initEasypay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EasyPays",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    WalletGateId = table.Column<string>(nullable: false),
                    IsWallet = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Price = table.Column<int>(maxLength: 15, nullable: false),
                    Text = table.Column<string>(maxLength: 250, nullable: false),
                    IsCoupon = table.Column<bool>(nullable: false),
                    IsUserEmail = table.Column<bool>(nullable: false),
                    IsUserName = table.Column<bool>(nullable: false),
                    IsUserPhone = table.Column<bool>(nullable: false),
                    IsUserText = table.Column<bool>(nullable: false),
                    IsUserEmailRequired = table.Column<bool>(nullable: false),
                    IsUserNameRequired = table.Column<bool>(nullable: false),
                    IsUserPhoneRequired = table.Column<bool>(nullable: false),
                    IsUserTextRequired = table.Column<bool>(nullable: false),
                    UserEmailExplain = table.Column<string>(maxLength: 25, nullable: false),
                    UserNameExplain = table.Column<string>(maxLength: 25, nullable: false),
                    UserPhoneExplain = table.Column<string>(maxLength: 25, nullable: false),
                    UserTextExplain = table.Column<string>(maxLength: 25, nullable: false),
                    IsCountLimit = table.Column<bool>(nullable: false),
                    CountLimit = table.Column<bool>(nullable: false),
                    ReturnSuccess = table.Column<bool>(nullable: false),
                    ReturnFail = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EasyPays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EasyPays_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EasyPays_UserId",
                table: "EasyPays",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EasyPays");
        }
    }
}
