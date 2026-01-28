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
            // 1) Drop old FKs only if they exist (avoids "Can't DROP ... doesn't exist")
            migrationBuilder.Sql(@"
-- Announcements -> Programs
SET @fk1 := (
  SELECT CONSTRAINT_NAME
  FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE()
    AND TABLE_NAME = 'Announcements'
    AND CONSTRAINT_NAME = 'FK_Announcements_Programs_ProgramId'
    AND CONSTRAINT_TYPE = 'FOREIGN KEY'
  LIMIT 1
);
SET @sql1 := IF(@fk1 IS NULL, 'SELECT 1;', CONCAT('ALTER TABLE `Announcements` DROP FOREIGN KEY `', @fk1, '`;'));
PREPARE stmt1 FROM @sql1; EXECUTE stmt1; DEALLOCATE PREPARE stmt1;

-- Announcements -> Teachers
SET @fk2 := (
  SELECT CONSTRAINT_NAME
  FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE()
    AND TABLE_NAME = 'Announcements'
    AND CONSTRAINT_NAME = 'FK_Announcements_Teachers_TeacherId'
    AND CONSTRAINT_TYPE = 'FOREIGN KEY'
  LIMIT 1
);
SET @sql2 := IF(@fk2 IS NULL, 'SELECT 1;', CONCAT('ALTER TABLE `Announcements` DROP FOREIGN KEY `', @fk2, '`;'));
PREPARE stmt2 FROM @sql2; EXECUTE stmt2; DEALLOCATE PREPARE stmt2;

-- StudentProgram -> Programs
SET @fk3 := (
  SELECT CONSTRAINT_NAME
  FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE()
    AND TABLE_NAME = 'StudentProgram'
    AND CONSTRAINT_NAME = 'FK_StudentProgram_Programs_ProgramId'
    AND CONSTRAINT_TYPE = 'FOREIGN KEY'
  LIMIT 1
);
SET @sql3 := IF(@fk3 IS NULL, 'SELECT 1;', CONCAT('ALTER TABLE `StudentProgram` DROP FOREIGN KEY `', @fk3, '`;'));
PREPARE stmt3 FROM @sql3; EXECUTE stmt3; DEALLOCATE PREPARE stmt3;

-- StudentProgram -> Students
SET @fk4 := (
  SELECT CONSTRAINT_NAME
  FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE()
    AND TABLE_NAME = 'StudentProgram'
    AND CONSTRAINT_NAME = 'FK_StudentProgram_Students_StudentId'
    AND CONSTRAINT_TYPE = 'FOREIGN KEY'
  LIMIT 1
);
SET @sql4 := IF(@fk4 IS NULL, 'SELECT 1;', CONCAT('ALTER TABLE `StudentProgram` DROP FOREIGN KEY `', @fk4, '`;'));
PREPARE stmt4 FROM @sql4; EXECUTE stmt4; DEALLOCATE PREPARE stmt4;

-- Subjects -> Programs
SET @fk5 := (
  SELECT CONSTRAINT_NAME
  FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE()
    AND TABLE_NAME = 'Subjects'
    AND CONSTRAINT_NAME = 'FK_Subjects_Programs_ProgramId'
    AND CONSTRAINT_TYPE = 'FOREIGN KEY'
  LIMIT 1
);
SET @sql5 := IF(@fk5 IS NULL, 'SELECT 1;', CONCAT('ALTER TABLE `Subjects` DROP FOREIGN KEY `', @fk5, '`;'));
PREPARE stmt5 FROM @sql5; EXECUTE stmt5; DEALLOCATE PREPARE stmt5;

-- Subjects -> Teachers
SET @fk6 := (
  SELECT CONSTRAINT_NAME
  FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE()
    AND TABLE_NAME = 'Subjects'
    AND CONSTRAINT_NAME = 'FK_Subjects_Teachers_TeacherId'
    AND CONSTRAINT_TYPE = 'FOREIGN KEY'
  LIMIT 1
);
SET @sql6 := IF(@fk6 IS NULL, 'SELECT 1;', CONCAT('ALTER TABLE `Subjects` DROP FOREIGN KEY `', @fk6, '`;'));
PREPARE stmt6 FROM @sql6; EXECUTE stmt6; DEALLOCATE PREPARE stmt6;
");

            // 2) Fix StudentProgram rename safely (avoids "table doesn't exist" + Pomelo PK helper crash)
            migrationBuilder.Sql(@"
-- If StudentProgram exists (old name), convert it to StudentPrograms (new name).
SET @has_old := (
  SELECT COUNT(*)
  FROM information_schema.TABLES
  WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'StudentProgram'
);

SET @has_new := (
  SELECT COUNT(*)
  FROM information_schema.TABLES
  WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'StudentPrograms'
);

-- Drop PK on StudentProgram (only if old table exists)
SET @sql := IF(@has_old = 1, 'ALTER TABLE `StudentProgram` DROP PRIMARY KEY;', 'SELECT 1;');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

-- Drop index IX_StudentProgram_StudentId if it exists (only if old table exists)
SET @idx := (
  SELECT INDEX_NAME
  FROM information_schema.STATISTICS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'StudentProgram'
    AND INDEX_NAME = 'IX_StudentProgram_StudentId'
  LIMIT 1
);
SET @sql := IF(@has_old = 1 AND @idx IS NOT NULL, 'DROP INDEX `IX_StudentProgram_StudentId` ON `StudentProgram`;', 'SELECT 1;');
PREPARE s2 FROM @sql; EXECUTE s2; DEALLOCATE PREPARE s2;

-- Rename table StudentProgram -> StudentPrograms (only if old exists AND new doesn't)
SET @sql := IF(@has_old = 1 AND @has_new = 0, 'RENAME TABLE `StudentProgram` TO `StudentPrograms`;', 'SELECT 1;');
PREPARE s3 FROM @sql; EXECUTE s3; DEALLOCATE PREPARE s3;

-- Ensure index name on ProgramId is IX_StudentPrograms_ProgramId (drop old, create new)
SET @idx_old_prog := (
  SELECT INDEX_NAME
  FROM information_schema.STATISTICS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'StudentPrograms'
    AND INDEX_NAME = 'IX_StudentProgram_ProgramId'
  LIMIT 1
);
SET @sql := IF(@idx_old_prog IS NOT NULL, 'DROP INDEX `IX_StudentProgram_ProgramId` ON `StudentPrograms`;', 'SELECT 1;');
PREPARE s4 FROM @sql; EXECUTE s4; DEALLOCATE PREPARE s4;

SET @idx_new_prog := (
  SELECT INDEX_NAME
  FROM information_schema.STATISTICS
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'StudentPrograms'
    AND INDEX_NAME = 'IX_StudentPrograms_ProgramId'
  LIMIT 1
);
SET @sql := IF(@idx_new_prog IS NULL, 'CREATE INDEX `IX_StudentPrograms_ProgramId` ON `StudentPrograms` (`ProgramId`);', 'SELECT 1;');
PREPARE s5 FROM @sql; EXECUTE s5; DEALLOCATE PREPARE s5;

-- Ensure composite primary key (StudentId, ProgramId) exists on StudentPrograms
SET @pkcols := (
  SELECT COUNT(*)
  FROM information_schema.KEY_COLUMN_USAGE
  WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'StudentPrograms'
    AND CONSTRAINT_NAME = 'PRIMARY'
);
SET @sql := IF(@pkcols = 0, 'ALTER TABLE `StudentPrograms` ADD PRIMARY KEY (`StudentId`,`ProgramId`);', 'SELECT 1;');
PREPARE s6 FROM @sql; EXECUTE s6; DEALLOCATE PREPARE s6;
");

            // 3) Auth tables
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

            // 4) IMPORTANT: TeacherId must be nullable for ON DELETE SET NULL
            migrationBuilder.AlterColumn<Guid>(
                name: "TeacherId",
                table: "Announcements",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldCollation: "ascii_general_ci");

            // 5) Re-add FKs in the desired shape
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
            // Drop FKs safely (if you ever rollback)
            migrationBuilder.Sql(@"
-- Announcements -> Programs
SET @d1 := (SELECT CONSTRAINT_NAME FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE() AND TABLE_NAME='Announcements'
    AND CONSTRAINT_NAME='FK_Announcements_Programs_ProgramId' AND CONSTRAINT_TYPE='FOREIGN KEY' LIMIT 1);
SET @q1 := IF(@d1 IS NULL,'SELECT 1;', CONCAT('ALTER TABLE `Announcements` DROP FOREIGN KEY `',@d1,'`;'));
PREPARE a1 FROM @q1; EXECUTE a1; DEALLOCATE PREPARE a1;

-- Announcements -> Teachers
SET @d2 := (SELECT CONSTRAINT_NAME FROM information_schema.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_SCHEMA = DATABASE() AND TABLE_NAME='Announcements'
    AND CONSTRAINT_NAME='FK_Announcements_Teachers_TeacherId' AND CONSTRAINT_TYPE='FOREIGN KEY' LIMIT 1);
SET @q2 := IF(@d2 IS NULL,'SELECT 1;', CONCAT('ALTER TABLE `Announcements` DROP FOREIGN KEY `',@d2,'`;'));
PREPARE a2 FROM @q2; EXECUTE a2; DEALLOCATE PREPARE a2;
");

            migrationBuilder.DropTable(name: "RefreshTokens");
            migrationBuilder.DropTable(name: "Users");

            // revert TeacherId to NOT NULL (original state)
            migrationBuilder.AlterColumn<Guid>(
                name: "TeacherId",
                table: "Announcements",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldCollation: "ascii_general_ci");

            // optional: rename StudentPrograms back only if needed
            migrationBuilder.Sql(@"
SET @has_new := (
  SELECT COUNT(*) FROM information_schema.TABLES
  WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'StudentPrograms'
);
SET @has_old := (
  SELECT COUNT(*) FROM information_schema.TABLES
  WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'StudentProgram'
);
SET @sql := IF(@has_new = 1 AND @has_old = 0, 'RENAME TABLE `StudentPrograms` TO `StudentProgram`;', 'SELECT 1;');
PREPARE r1 FROM @sql; EXECUTE r1; DEALLOCATE PREPARE r1;
");
        }
    }
}
