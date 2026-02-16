using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPrimaryProgramToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryProgramId",
                table: "Students",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Students_PrimaryProgramId",
                table: "Students",
                column: "PrimaryProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Programs_PrimaryProgramId",
                table: "Students",
                column: "PrimaryProgramId",
                principalTable: "Programs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Programs_PrimaryProgramId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_PrimaryProgramId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PrimaryProgramId",
                table: "Students");
        }
    }
}
