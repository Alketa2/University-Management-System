using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using UniversityManagement.Domain.MongoEntities;

namespace UniversityManagement.Infrastructure.Data.Mongo;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("MongoDb:ConnectionString").Value ?? "mongodb://localhost:27017";
        var databaseName = configuration.GetSection("MongoDb:DatabaseName").Value ?? "UniversityManagementMongo";
        
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Notification> Notifications => 
        _database.GetCollection<Notification>("Notifications");

    public IMongoCollection<CourseResource> CourseResources => 
        _database.GetCollection<CourseResource>("CourseResources");
}
