using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniversityManagement.Domain.MongoEntities;

public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "General"; // Info, Warning, Error, Success
    
    public string RecipientId { get; set; } = string.Empty; // User ID (Guid as string)
    public string RecipientRole { get; set; } = string.Empty; // Student, Teacher, Admin
    
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Flexible data field for deep linking or extra info
    public Dictionary<string, string> Metadata { get; set; } = new();
}
