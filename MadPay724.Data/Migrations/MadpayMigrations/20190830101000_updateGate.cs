using Microsoft.EntityFrameworkCore.Migrations;

namespace MadPay724.Data.Migrations.MadpayMigrations
{
    public partial class updateGate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gate_Wallets_WalletId",
                table: "Gate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gate",
                table: "Gate");

            migrationBuilder.RenameTable(
                name: "Gate",
                newName: "Gates");

            migrationBuilder.RenameIndex(
                name: "IX_Gate_WalletId",
                table: "Gates",
                newName: "IX_Gates_WalletId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Gates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDirect",
                table: "Gates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gates",
                table: "Gates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Gates_Wallets_WalletId",
                table: "Gates",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gates_Wallets_WalletId",
                table: "Gates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gates",
                table: "Gates");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Gates");

            migrationBuilder.DropColumn(
                name: "IsDirect",
                table: "Gates");

            migrationBuilder.RenameTable(
                name: "Gates",
                newName: "Gate");

            migrationBuilder.RenameIndex(
                name: "IX_Gates_WalletId",
                table: "Gate",
                newName: "IX_Gate_WalletId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gate",
                table: "Gate",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Gate_Wallets_WalletId",
                table: "Gate",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
