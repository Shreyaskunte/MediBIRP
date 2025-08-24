using Shared.Models;

namespace NoteService.Services
{
    public interface INoteRepository
    {
        Task<NoteDocument> CreateAsync(NoteDocument note);
        Task<NoteDocument?> GetAsync(string id);
        Task<IEnumerable<NoteDocument>> ListByPatientAsync(string patientId);
        Task UpdateAsync(NoteDocument note);
    }
}
