using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Api.Seeding;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config, ILogger logger)
    {
        var section = config.GetSection("SeedAdmin");
        var enabled = section.GetValue<bool>("Enabled");

        if (!enabled)
        {
            logger.LogInformation("Admin seeding is disabled (SeedAdmin:Enabled=false).");
            return;
        }

        var email = section.GetValue<string>("Email");
        var password = section.GetValue<string>("Password");
        var firstName = section.GetValue<string>("FirstName") ?? "System";
        var lastName = section.GetValue<string>("LastName") ?? "Admin";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Admin seeding skipped: SeedAdmin:Email or SeedAdmin:Password is missing.");
            return;
        }

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();

        //  DB is created/
        await db.Database.MigrateAsync();

        
        var existing = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existing != null)
        {
            logger.LogInformation("Admin user already exists: {Email}", email);
            return;
        }

        var hasher = new PasswordHasher<AppUser>();

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = "Admin", 
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        user.PasswordHash = hasher.HashPassword(user, password);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        logger.LogInformation("Seeded Admin user: {Email}", email);
    }
}
