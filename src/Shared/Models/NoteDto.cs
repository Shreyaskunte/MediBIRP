namespace Shared.Models
{
    public class CreateNoteDto
    {
        public string PatientId { get; set; } = default!;
        public string ClinicianId { get; set; } = default!;
        public string RawText { get; set; } = default!;
        public DateTime NoteDate { get; set; } = DateTime.UtcNow;
    }

    public class ParsedBirp
    {
        public string Behavior { get; set; } = "";
        public string Intervention { get; set; } = "";
        public string Response { get; set; } = "";
        public string Plan { get; set; } = "";
    }

    public class NoteDocument
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PatientId { get; set; } = default!;
        public string ClinicianId { get; set; } = default!;
        public string RawText { get; set; } = default!;
        public ParsedBirp Parsed { get; set; } = new ParsedBirp();
        public DateTime NoteDate { get; set; } = DateTime.UtcNow;
        public string? PdfUrl { get; set; }
    }
}
