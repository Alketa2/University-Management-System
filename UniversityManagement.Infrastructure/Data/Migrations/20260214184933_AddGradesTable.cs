using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGradesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Conditionally drop foreign keys if they exist
            migrationBuilder.Sql(@"
                SET @constraintExists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'Announcements' 
                    AND CONSTRAINT_NAME = 'FK_Announcements_Programs_ProgramId'
                    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
                
                SET @sqlStatement = IF(@constraintExists > 0, 
                    'ALTER TABLE `Announcements` DROP FOREIGN KEY `FK_Announcements_Programs_ProgramId`', 
                    'SELECT ''Constraint does not exist'' AS msg');
                
                PREPARE stmt FROM @sqlStatement;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @constraintExists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'Announcements' 
                    AND CONSTRAINT_NAME = 'FK_Announcements_Teachers_TeacherId'
                    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
                
                SET @sqlStatement = IF(@constraintExists > 0, 
                    'ALTER TABLE `Announcements` DROP FOREIGN KEY `FK_Announcements_Teachers_TeacherId`', 
                    'SELECT ''Constraint does not exist'' AS msg');
                
                PREPARE stmt FROM @sqlStatement;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @constraintExists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'Subjects' 
                    AND CONSTRAINT_NAME = 'FK_Subjects_Programs_ProgramId'
                    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
                
                SET @sqlStatement = IF(@constraintExists > 0, 
                    'ALTER TABLE `Subjects` DROP FOREIGN KEY `FK_Subjects_Programs_ProgramId`', 
                    'SELECT ''Constraint does not exist'' AS msg');
                
                PREPARE stmt FROM @sqlStatement;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @constraintExists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'Subjects' 
                    AND CONSTRAINT_NAME = 'FK_Subjects_Teachers_TeacherId'
                    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
                
                SET @sqlStatement = IF(@constraintExists > 0, 
                    'ALTER TABLE `Subjects` DROP FOREIGN KEY `FK_Subjects_Teachers_TeacherId`', 
                    'SELECT ''Constraint does not exist'' AS msg');
                
                PREPARE stmt FROM @sqlStatement;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Conditionally drop Attendances table if it exists
            migrationBuilder.Sql(@"
                SET @tableExists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'Attendances');
                
                SET @sqlStatement = IF(@tableExists > 0, 
                    'DROP TABLE `Attendances`', 
                    'SELECT ''Table does not exist'' AS msg');
                
                PREPARE stmt FROM @sqlStatement;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Drop StudentPrograms FK constraints before changing PK
            migrationBuilder.Sql(@"
                SET @constraintExists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'StudentPrograms' 
                    AND CONSTRAINT_NAME = 'FK_StudentPrograms_Programs_ProgramId'
                    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
                
                SET @sqlStatement = IF(@constraintExists > 0, 
                    'ALTER TABLE `StudentPrograms` DROP FOREIGN KEY `FK_StudentPrograms_Programs_ProgramId`', 
                    'SELECT ''Constraint does not exist'' AS msg');
                
                PREPARE stmt FROM @sqlStatement;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @constraintExists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'StudentPrograms' 
                    AND CONSTRAINT_NAME = 'FK_StudentPrograms_Students_StudentId'
                    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
                
                SET @sqlStatement = IF(@constraintExists > 0, 
                    'ALTER TABLE `StudentPrograms` DROP FOREIGN KEY `FK_StudentPrograms_Students_StudentId`', 
                    'SELECT ''Constraint does not exist'' AS msg');
                
                PREPARE stmt FROM @sqlStatement;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Drop and recreate PK on StudentPrograms
            migrationBuilder.Sql(@"
                -- Drop old composite PK if it exists
                SET @pkExists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'StudentPrograms' 
                    AND CONSTRAINT_TYPE = 'PRIMARY KEY');
                
                SET @sqlStatement = IF(@pkExists > 0, 
                    'ALTER TABLE `StudentPrograms` DROP PRIMARY KEY', 
                    'SELECT ''PK does not exist'' AS msg');
                
                PREPARE stmt FROM @sqlStatement;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Conditionally drop PublishDate column if it exists
            migrationBuilder.Sql(@"
                SET @columnExists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'Announcements' 
                    AND COLUMN_NAME = 'PublishDate');
                
                SET @sqlStatement = IF(@columnExists > 0, 
                    'ALTER TABLE `Announcements` DROP COLUMN `PublishDate`', 
                    'SELECT ''Column does not exist'' AS msg');
                
                PREPARE stmt FROM @sqlStatement;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");


            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldMaxLength: 30)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeacherId",
                table: "Announcements",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "TargetAudience",
                table: "Announcements",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Announcements",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(5000)",
                oldMaxLength: 5000)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            // Add new single-column PK on StudentPrograms (if not already exists)
            migrationBuilder.Sql(@"
                SET @pkExists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'StudentPrograms' 
                    AND CONSTRAINT_TYPE = 'PRIMARY KEY');
                
                SET @sqlStatement = IF(@pkExists = 0, 
                    'ALTER TABLE `StudentPrograms` ADD PRIMARY KEY (`Id`)', 
                    'SELECT ''PK already exists'' AS msg');
                
                PREPARE stmt FROM @sqlStatement;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");
            
            // Recreate FK constraints for StudentPrograms
            migrationBuilder.Sql(@"
                SET @constraintExists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'StudentPrograms' 
                    AND CONSTRAINT_NAME = 'FK_StudentPrograms_Students_StudentId'
                    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
                
                SET @sqlStatement = IF(@constraintExists = 0, 
                    'ALTER TABLE `StudentPrograms` ADD CONSTRAINT `FK_StudentPrograms_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `Students` (`Id`) ON DELETE CASCADE', 
                    'SELECT ''FK already exists'' AS msg');
                
                PREPARE stmt FROM @sqlStatement;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @constraintExists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_SCHEMA = DATABASE() 
                    AND TABLE_NAME = 'StudentPrograms' 
                    AND CONSTRAINT_NAME = 'FK_StudentPrograms_Programs_ProgramId'
                    AND CONSTRAINT_TYPE = 'FOREIGN KEY');
                
                SET @sqlStatement = IF(@constraintExists = 0, 
                    'ALTER TABLE `StudentPrograms` ADD CONSTRAINT `FK_StudentPrograms_Programs_ProgramId` FOREIGN KEY (`ProgramId`) REFERENCES `Programs` (`Id`) ON DELETE CASCADE', 
                    'SELECT ''FK already exists'' AS msg');
                
                PREPARE stmt FROM @sqlStatement;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StudentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SubjectId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ExamId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Score = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    LetterGrade = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Percentage = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    GradePoint = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Comments = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GradedByTeacherId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    AcademicYear = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grades_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Grades_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grades_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grades_Teachers_GradedByTeacherId",
                        column: x => x.GradedByTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPrograms_StudentId",
                table: "StudentPrograms",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_ExamId",
                table: "Grades",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_GradedByTeacherId",
                table: "Grades",
                column: "GradedByTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_StudentId",
                table: "Grades",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_SubjectId",
                table: "Grades",
                column: "SubjectId");

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
                principalColumn: "Id");

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
                name: "FK_Subjects_Programs_ProgramId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Teachers_TeacherId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentPrograms",
                table: "StudentPrograms");

            migrationBuilder.DropIndex(
                name: "IX_StudentPrograms_StudentId",
                table: "StudentPrograms");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeacherId",
                table: "Announcements",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "TargetAudience",
                table: "Announcements",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Announcements",
                type: "varchar(5000)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "Announcements",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentPrograms",
                table: "StudentPrograms",
                columns: new[] { "StudentId", "ProgramId" });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StudentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SubjectId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AttendanceDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendances_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendances",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SubjectId",
                table: "Attendances",
                column: "SubjectId");

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
    }
}
