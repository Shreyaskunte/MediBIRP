using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NoteService.Models;
using Shared.DTO.Notes;

namespace NoteService.Services
{
    public class NoteServiceImpl : INoteService
    {
        private readonly IMongoCollection<Note> _notes;

        public NoteServiceImpl(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _notes = database.GetCollection<Note>(settings.Value.CollectionName);
        }

        public async Task<List<NoteDto>> GetAsync()
        {
            var notes = await _notes.Find(_ => true).ToListAsync();
            return notes.Select(n => new NoteDto(n.Id, n.Title, n.Content, n.CreatedAt)).ToList();
        }

        public async Task<NoteDto?> GetAsync(string id)
        {
            var note = await _notes.Find(x => x.Id == id).FirstOrDefaultAsync();
            return note is null ? null : new NoteDto(note.Id, note.Title, note.Content, note.CreatedAt);
        }

        public async Task<NoteDto> CreateAsync(CreateNoteDto dto, string clinicianId)
        {
            var note = new Note
            {
                ClinicianId = clinicianId,
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _notes.InsertOneAsync(note);
            return new NoteDto(note.Id, note.Title, note.Content, note.CreatedAt);
        }

        public async Task<NoteDto?> UpdateAsync(string id, UpdateNoteDto dto)
        {
            var note = await _notes.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (note is null) return null;

            note.Title = dto.Title;
            note.Content = dto.Content;

            await _notes.ReplaceOneAsync(x => x.Id == id, note);

            return new NoteDto(note.Id, note.Title, note.Content, note.CreatedAt);
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var result = await _notes.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }
    }

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string CollectionName { get; set; } = string.Empty;
    }
}
