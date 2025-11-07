using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teklif_Sepeti.Migrations
{
    /// <inheritdoc />
    public partial class LinkProposalsToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Proposals",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_ApplicationUserId",
                table: "Proposals",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_AspNetUsers_ApplicationUserId",
                table: "Proposals",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_AspNetUsers_ApplicationUserId",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_ApplicationUserId",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Proposals");
        }
    }
}
