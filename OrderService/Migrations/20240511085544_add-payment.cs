﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Migrations
{
    /// <inheritdoc />
    public partial class addpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Orders");
        }
    }
}
