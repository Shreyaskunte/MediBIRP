using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NoteService.Models
{
    public class Note
    {
        [BsonId] // MongoDB ObjectId
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string ClinicianId { get; set; } = string.Empty; // from JWT
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
