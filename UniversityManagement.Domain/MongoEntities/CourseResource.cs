using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniversityManagement.Domain.MongoEntities;

public class CourseResource
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string SubjectId { get; set; } = string.Empty; // Guid as string
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public string ResourceType { get; set; } = "Link"; // PDF, Video, Link, Quiz, Document
    public string Url { get; set; } = string.Empty;
    
    public string UploadedBy { get; set; } = string.Empty; // Teacher ID
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // This allows teachers to add custom properties
    public Dictionary<string, string>? ExtraData { get; set; }
}
