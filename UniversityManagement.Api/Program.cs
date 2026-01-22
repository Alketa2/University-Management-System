using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Application.Services;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Interfaces;
using UniversityManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

//  configuring Swagger     
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "University Management System API",
        Version = "v1",
        Description = "API for managing university students, programs, teachers, subjects, attendance, exams, timetables, and announcements"
    });

    // Swagger documentation
    c.MapType<Microsoft.AspNetCore.Mvc.ProblemDetails>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiSchema>
        {
            ["type"] = new Microsoft.OpenApi.Models.OpenApiSchema { Type = "string", Nullable = true, Description = "A URI reference that identifies the problem type" },
            ["title"] = new Microsoft.OpenApi.Models.OpenApiSchema { Type = "string", Nullable = true, Description = "A short, human-readable summary of the problem type" },
            ["status"] = new Microsoft.OpenApi.Models.OpenApiSchema { Type = "integer", Format = "int32", Nullable = true, Description = "The HTTP status code" },
            ["detail"] = new Microsoft.OpenApi.Models.OpenApiSchema { Type = "string", Nullable = true, Description = "A human-readable explanation specific to this occurrence of the problem" },
            ["instance"] = new Microsoft.OpenApi.Models.OpenApiSchema { Type = "string", Nullable = true, Description = "A URI reference that identifies the specific occurrence of the problem" },
            ["errors"] = new Microsoft.OpenApi.Models.OpenApiSchema
            {
                Type = "object",
                Description = "Validation errors (only present for 400 Bad Request with validation errors)",
                AdditionalProperties = new Microsoft.OpenApi.Models.OpenApiSchema
                {
                    Type = "array",
                    Items = new Microsoft.OpenApi.Models.OpenApiSchema { Type = "string" }
                }
            }
        }
    });


    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // - "schemaId" collisions 
    c.CustomSchemaIds(type => type.FullName);
});

//  CORS for React frontend
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

// Register Repositories
builder.Services.AddSingleton<IRepository<Student>, StudentRepository>();
builder.Services.AddSingleton<IRepository<UniversityManagement.Domain.Entities.Program>, ProgramRepository>();
builder.Services.AddSingleton<IRepository<Teacher>, TeacherRepository>();
builder.Services.AddSingleton<IRepository<Subject>, SubjectRepository>();
builder.Services.AddSingleton<IRepository<Attendance>, AttendanceRepository>();
builder.Services.AddSingleton<IRepository<Exam>, ExamRepository>();
builder.Services.AddSingleton<IRepository<Timetable>, TimetableRepository>();
builder.Services.AddSingleton<IRepository<Announcement>, AnnouncementRepository>();

// Register specific repositories
builder.Services.AddSingleton<IStudentRepository, StudentRepository>();
builder.Services.AddSingleton<IStudentProgramRepository, StudentProgramRepository>();
builder.Services.AddSingleton<ISubjectRepository, SubjectRepository>();
builder.Services.AddSingleton<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddSingleton<IExamRepository, ExamRepository>();
builder.Services.AddSingleton<ITimetableRepository, TimetableRepository>();
builder.Services.AddSingleton<IAnnouncementRepository, AnnouncementRepository>();

// Register Services
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
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "University Management System API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
