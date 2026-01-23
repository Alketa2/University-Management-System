using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Application.Services;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Infrastructure.Data;
using UniversityManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Validation response
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var result = new BadRequestObjectResult(context.ModelState);
            result.ContentTypes.Add("application/json");
            return result;
        };
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ✅ DbContext (Pomelo MySQL)
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UniversityDbContext>(options =>
{
    options.UseMySql(cs, ServerVersion.AutoDetect(cs));
});

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentProgramRepository, StudentProgramRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();

// Services
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IProgramService, ProgramService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<ITimetableService, TimetableService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
