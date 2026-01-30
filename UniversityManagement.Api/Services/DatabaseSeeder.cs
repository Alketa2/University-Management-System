using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Api.Services;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();

        if (await db.Programs.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;

        var programCsId = Guid.Parse("8a7e5d2c-7f2f-4f25-8d7f-bf0e6b2764a1");
        var programBizId = Guid.Parse("1b27945a-b2cb-4a2f-9c7f-9ef8e82f3e2b");
        var teacherOneId = Guid.Parse("b9e7a1a4-1c0c-4f9b-9d6a-9e4b4f69b82b");
        var teacherTwoId = Guid.Parse("9a5f0d44-6f28-4e4d-9c6d-5f1c7a5cfe54");
        var subjectAlgoId = Guid.Parse("0fca5e1f-1bb3-4b30-8a56-90f91b25f6c1");
        var subjectAcctId = Guid.Parse("7d4028f5-5f1f-4fe5-96a4-9a0b5f2a3f9d");
        var studentOneId = Guid.Parse("3a2af6d1-31f6-48d4-90ab-2a7c1d9b8a6f");
        var studentTwoId = Guid.Parse("f4b1d85c-92fd-4b2b-9f4c-9c0c5f1f0a3d");
        var adminUserId = Guid.Parse("45a69f7c-8b8a-4a7c-bb3b-992a88920b6a");

        var programs = new[]
        {
            new Program
            {
                Id = programCsId,
                Name = "Computer Science",
                Code = "CS",
                Description = "Software engineering and data systems program.",
                Duration = 8,
                CreditsRequired = 120,
                StartDate = now.AddYears(-2),
                IsActive = true,
                CreatedAt = now.AddYears(-2)
            },
            new Program
            {
                Id = programBizId,
                Name = "Business Administration",
                Code = "BBA",
                Description = "Business management and finance program.",
                Duration = 8,
                CreditsRequired = 120,
                StartDate = now.AddYears(-1),
                IsActive = true,
                CreatedAt = now.AddYears(-1)
            }
        };

        var teachers = new[]
        {
            new Teacher
            {
                Id = teacherOneId,
                FirstName = "Amina",
                LastName = "Khan",
                Email = "amina.khan@university.local",
                Department = "Computer Science",
                HireDate = now.AddYears(-5),
                Status = TeacherStatus.Active,
                CreatedAt = now.AddYears(-5)
            },
            new Teacher
            {
                Id = teacherTwoId,
                FirstName = "David",
                LastName = "Lee",
                Email = "david.lee@university.local",
                Department = "Business",
                HireDate = now.AddYears(-3),
                Status = TeacherStatus.Active,
                CreatedAt = now.AddYears(-3)
            }
        };

        var subjects = new[]
        {
            new Subject
            {
                Id = subjectAlgoId,
                Name = "Algorithms",
                Code = "CS201",
                Description = "Algorithm design and analysis.",
                Credits = 4,
                ProgramId = programCsId,
                TeacherId = teacherOneId,
                Semester = 3,
                IsActive = true,
                CreatedAt = now.AddMonths(-8)
            },
            new Subject
            {
                Id = subjectAcctId,
                Name = "Financial Accounting",
                Code = "BBA210",
                Description = "Foundations of accounting and reporting.",
                Credits = 3,
                ProgramId = programBizId,
                TeacherId = teacherTwoId,
                Semester = 2,
                IsActive = true,
                CreatedAt = now.AddMonths(-6)
            }
        };

        var students = new[]
        {
            new Student
            {
                Id = studentOneId,
                FirstName = "Noah",
                LastName = "Patel",
                Email = "noah.patel@student.local",
                Phone = "555-0101",
                DateOfBirth = new DateTime(2003, 5, 12),
                Address = "101 College Ave",
                EnrollmentDate = now.AddMonths(-10),
                Status = StudentStatus.Active,
                CreatedAt = now.AddMonths(-10)
            },
            new Student
            {
                Id = studentTwoId,
                FirstName = "Sofia",
                LastName = "Martinez",
                Email = "sofia.martinez@student.local",
                Phone = "555-0112",
                DateOfBirth = new DateTime(2004, 2, 3),
                Address = "202 University Blvd",
                EnrollmentDate = now.AddMonths(-8),
                Status = StudentStatus.Enrolled,
                CreatedAt = now.AddMonths(-8)
            }
        };

        var studentPrograms = new[]
        {
            new StudentProgram
            {
                Id = Guid.Parse("29c3c1fb-2c33-49d6-8e1c-1d0f5762c58d"),
                StudentId = studentOneId,
                ProgramId = programCsId,
                AdmissionDate = now.AddMonths(-10),
                CreatedAt = now.AddMonths(-10)
            },
            new StudentProgram
            {
                Id = Guid.Parse("5b0f50b9-b4a1-4f6d-9bda-5f038a0039c4"),
                StudentId = studentTwoId,
                ProgramId = programBizId,
                AdmissionDate = now.AddMonths(-8),
                CreatedAt = now.AddMonths(-8)
            }
        };

        var attendance = new[]
        {
            new Attendance
            {
                Id = Guid.Parse("d444ea41-8c23-4d67-9d8a-1fd7b1f4f0a3"),
                StudentId = studentOneId,
                SubjectId = subjectAlgoId,
                AttendanceDate = now.AddDays(-2),
                Status = AttendanceStatus.Present,
                Notes = "On time",
                CreatedAt = now.AddDays(-2)
            }
        };

        var exams = new[]
        {
            new Exam
            {
                Id = Guid.Parse("b68d04fa-8a3f-4c4f-96e9-6678079d1ed1"),
                Name = "Algorithms Midterm",
                ExamType = ExamType.Midterm,
                SubjectId = subjectAlgoId,
                ExamDate = now.AddDays(14).Date,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(11, 0, 0),
                Location = "Room 204",
                MaxMarks = 100,
                CreatedAt = now
            }
        };

        var timetables = new[]
        {
            new Timetable
            {
                Id = Guid.Parse("af33a2f9-23d2-40b7-9d12-645f444ecfa4"),
                ProgramId = programCsId,
                SubjectId = subjectAlgoId,
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(12, 0, 0),
                Room = "Lab 3",
                Semester = 3,
                AcademicYear = $"{now.Year}/{now.Year + 1}",
                CreatedAt = now.AddMonths(-1)
            }
        };

        var announcements = new[]
        {
            new Announcement
            {
                Id = Guid.Parse("e0a7a8b4-5a24-4e38-9d41-15b8b9702df0"),
                Title = "Welcome Week",
                Content = "Orientation begins next Monday. Check your timetable for details.",
                TargetAudience = "All",
                ProgramId = programCsId,
                SubjectId = subjectAlgoId,
                TeacherId = teacherOneId,
                ExpiryDate = now.AddDays(30),
                IsActive = true,
                CreatedAt = now
            }
        };

        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<AppUser>>();
        var adminUser = new AppUser
        {
            Id = adminUserId,
            Email = "admin@university.local",
            Role = "Admin",
            IsActive = true,
            CreatedAt = now
        };
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");

        db.Programs.AddRange(programs);
        db.Teachers.AddRange(teachers);
        db.Subjects.AddRange(subjects);
        db.Students.AddRange(students);
        db.StudentPrograms.AddRange(studentPrograms);
        db.Attendances.AddRange(attendance);
        db.Exams.AddRange(exams);
        db.Timetables.AddRange(timetables);
        db.Announcements.AddRange(announcements);
        db.Users.Add(adminUser);

        await db.SaveChangesAsync();
    }
}
