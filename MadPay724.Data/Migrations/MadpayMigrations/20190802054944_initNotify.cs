using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MadPay724.Data.Migrations.MadpayMigrations
{
    public partial class initNotify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    EnterEmail = table.Column<bool>(nullable: false),
                    EnterSms = table.Column<bool>(nullable: false),
                    EnterTelegram = table.Column<bool>(nullable: false),
                    ExitEmail = table.Column<bool>(nullable: false),
                    ExitSms = table.Column<bool>(nullable: false),
                    ExitTelegram = table.Column<bool>(nullable: false),
                    TicketEmail = table.Column<bool>(nullable: false),
                    TicketSms = table.Column<bool>(nullable: false),
                    TicketTelegram = table.Column<bool>(nullable: false),
                    LoginEmail = table.Column<bool>(nullable: false),
                    LoginSms = table.Column<bool>(nullable: false),
                    LoginTelegram = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
