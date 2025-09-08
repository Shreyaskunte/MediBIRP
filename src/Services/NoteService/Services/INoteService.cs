using NoteService.Models;
using Shared.DTO.Notes;

namespace NoteService.Services
{
    public interface INoteService
    {
        Task<List<NoteDto>> GetAsync();
        Task<NoteDto?> GetAsync(string id);
        Task<NoteDto> CreateAsync(CreateNoteDto dto, string clinicianId);
        Task<NoteDto?> UpdateAsync(string id, UpdateNoteDto dto);
        Task<bool> RemoveAsync(string id);
    }
}
