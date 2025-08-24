using MongoDB.Driver;
using Shared.Models;

namespace NoteService.Services
{
    public class NoteRepository : INoteRepository
    {
        private readonly IMongoCollection<NoteDocument> _col;
        public NoteRepository(IMongoClient client)
        {
            var db = client.GetDatabase("care_notes");
            _col = db.GetCollection<NoteDocument>("notes");
        }

        public async Task<NoteDocument> CreateAsync(NoteDocument note)
        {
            await _col.InsertOneAsync(note);
            return note;
        }

        public async Task<NoteDocument?> GetAsync(string id) =>
            await _col.Find(n => n.Id == id).FirstOrDefaultAsync();

        public async Task<IEnumerable<NoteDocument>> ListByPatientAsync(string patientId) =>
            await _col.Find(n => n.PatientId == patientId).ToListAsync();

        public async Task UpdateAsync(NoteDocument note) =>
            await _col.ReplaceOneAsync(n => n.Id == note.Id, note);
    }
}
