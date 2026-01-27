using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Programs_ProgramId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Teachers_TeacherId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProgram_Programs_ProgramId",
                table: "StudentProgram");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProgram_Students_StudentId",
                table: "StudentProgram");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Programs_ProgramId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Teachers_TeacherId",
                table: "Subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentProgram",
                table: "StudentProgram");

            migrationBuilder.DropIndex(
                name: "IX_StudentProgram_StudentId",
                table: "StudentProgram");

            migrationBuilder.RenameTable(
                name: "StudentProgram",
                newName: "StudentPrograms");

            migrationBuilder.RenameIndex(
                name: "IX_StudentProgram_ProgramId",
                table: "StudentPrograms",
                newName: "IX_StudentPrograms_ProgramId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentPrograms",
                table: "StudentPrograms",
                columns: new[] { "StudentId", "ProgramId" });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AppUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Token = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_AppUserId",
                table: "RefreshTokens",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Programs_ProgramId",
                table: "Announcements",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Teachers_TeacherId",
                table: "Announcements",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentPrograms_Programs_ProgramId",
                table: "StudentPrograms",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentPrograms_Students_StudentId",
                table: "StudentPrograms",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Programs_ProgramId",
                table: "Subjects",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Teachers_TeacherId",
                table: "Subjects",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Programs_ProgramId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Teachers_TeacherId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentPrograms_Programs_ProgramId",
                table: "StudentPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentPrograms_Students_StudentId",
                table: "StudentPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Programs_ProgramId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Teachers_TeacherId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentPrograms",
                table: "StudentPrograms");

            migrationBuilder.RenameTable(
                name: "StudentPrograms",
                newName: "StudentProgram");

            migrationBuilder.RenameIndex(
                name: "IX_StudentPrograms_ProgramId",
                table: "StudentProgram",
                newName: "IX_StudentProgram_ProgramId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentProgram",
                table: "StudentProgram",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProgram_StudentId",
                table: "StudentProgram",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Programs_ProgramId",
                table: "Announcements",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Teachers_TeacherId",
                table: "Announcements",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProgram_Programs_ProgramId",
                table: "StudentProgram",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProgram_Students_StudentId",
                table: "StudentProgram",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Programs_ProgramId",
                table: "Subjects",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Teachers_TeacherId",
                table: "Subjects",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
