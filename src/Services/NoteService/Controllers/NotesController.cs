using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteService.Services;
using Shared.Models;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

namespace NoteService.Controllers
{
    [ApiController]
    [Route("api/notes")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INoteRepository _repo;
        private readonly INoteParser _parser;
        private readonly IConfiguration _config;
        public NotesController(INoteRepository repo, INoteParser parser, IConfiguration config)
        {
            _repo = repo; _parser = parser; _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNoteDto dto)
        {
            var note = new NoteDocument
            {
                PatientId = dto.PatientId,
                ClinicianId = dto.ClinicianId,
                RawText = dto.RawText,
                NoteDate = dto.NoteDate,
                Parsed = _parser.Parse(dto.RawText)
            };
            await _repo.CreateAsync(note);

            // publish message to RabbitMQ for PDF generation
            var factory = new ConnectionFactory() { Uri = new Uri(_config.GetConnectionString("RabbitMq") ?? "amqp://guest:guest@rabbitmq:5672/") };
            using var conn = factory.CreateConnection();
            using var ch = conn.CreateModel();
            ch.QueueDeclare("note_pdf_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { noteId = note.Id }));
            ch.BasicPublish("", "note_pdf_queue", null, body);

            return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var note = await _repo.GetAsync(id);
            return note == null ? NotFound() : Ok(note);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> ListForPatient(string patientId)
        {
            var list = await _repo.ListByPatientAsync(patientId);
            return Ok(list);
        }

        [HttpPost("{id}/export/pdf")]
        public async Task<IActionResult> ExportPdf(string id)
        {
            // Enqueue explicit PDF generation request
            var factory = new ConnectionFactory() { Uri = new Uri(_config.GetConnectionString("RabbitMq") ?? "amqp://guest:guest@rabbitmq:5672/") };
            using var conn = factory.CreateConnection();
            using var ch = conn.CreateModel();
            ch.QueueDeclare("note_pdf_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { noteId = id }));
            ch.BasicPublish("", "note_pdf_queue", null, body);
            return Ok(new { queued = true });
        }
    }
}
