using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Notes;
using NoteService.Services;
using System.Security.Claims;

namespace NoteService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require JWT for all endpoints
    public class NotesController : ControllerBase
    {
        private readonly INoteService _service;

        public NotesController(INoteService service)
        {
            _service = service;
        }

        // GET /api/notes
        [HttpGet]
        public async Task<ActionResult<List<NoteDto>>> GetAll()
        {
            var clinicianId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var notes = await _service.GetAsync(); // Later you can filter by clinicianId
            return Ok(notes);
        }

        // GET /api/notes/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<NoteDto>> Get(string id)
        {
            var note = await _service.GetAsync(id);
            if (note is null) return NotFound();
            return Ok(note);
        }

        // POST /api/notes
        [HttpPost]
        public async Task<ActionResult<NoteDto>> Create(CreateNoteDto dto)
        {
            var clinicianId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var note = await _service.CreateAsync(dto, clinicianId);
            return CreatedAtAction(nameof(Get), new { id = note.Id }, note);
        }

        // PUT /api/notes/{id}
        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult<NoteDto>> Update(string id, UpdateNoteDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null)  return NotFound();
            return Ok(updated);
        }

        // DELETE /api/notes/{id}
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _service.RemoveAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}