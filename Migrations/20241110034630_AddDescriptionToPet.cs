﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetAdoptionApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Pets");
        }
    }
}
