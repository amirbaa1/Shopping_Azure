using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatepayModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Authorithy",
                table: "payments");

            migrationBuilder.AddColumn<string>(
                name: "Authority",
                table: "payments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Authority",
                table: "payments");

            migrationBuilder.AddColumn<string>(
                name: "Authorithy",
                table: "payments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
