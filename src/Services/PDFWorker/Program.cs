using Azure.Storage.Blobs;
using MongoDB.Driver;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models;
using System.Text;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional:true).AddEnvironmentVariables().Build();

var mongoConn = config.GetConnectionString("Mongo") ?? "mongodb://mongo:27017";
var client = new MongoClient(mongoConn);
var db = client.GetDatabase("care_notes");
var notes = db.GetCollection<NoteDocument>("notes");

var factory = new ConnectionFactory() { Uri = new Uri(config.GetConnectionString("RabbitMq") ?? "amqp://guest:guest@rabbitmq:5672/") };
using var conn = factory.CreateConnection();
using var ch = conn.CreateModel();
ch.QueueDeclare("note_pdf_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

var consumer = new EventingBasicConsumer(ch);
consumer.Received += async (sender, ea) =>
{
    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
    dynamic obj = JsonConvert.DeserializeObject<object>(json)!;
    string noteId = obj.noteId;
    var note = await notes.Find(n => n.Id == noteId).FirstOrDefaultAsync();
    if (note == null) { ch.BasicAck(ea.DeliveryTag, false); return; }

    // Generate PDF via QuestPDF
    var fileName = $"{noteId}.pdf";
    using var ms = new MemoryStream();
    Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(20);
            page.Header().Text("CareNotes Clinic").Bold().FontSize(20);
            page.Content().Column(col =>
            {
                col.Item().Text($"Patient: {note.PatientId} | Clinician: {note.ClinicianId}").FontSize(12);
                col.Item().Text($"Date: {note.NoteDate:yyyy-MM-dd HH:mm}").FontSize(11);
                col.Item().Text("Behavior:").Bold();
                col.Item().Text(note.Parsed.Behavior);
                col.Item().Text("Intervention:").Bold();
                col.Item().Text(note.Parsed.Intervention);
                col.Item().Text("Response:").Bold();
                col.Item().Text(note.Parsed.Response);
                col.Item().Text("Plan:").Bold();
                col.Item().Text(note.Parsed.Plan);
            });
        });
    }).GeneratePdf(ms);

    ms.Position = 0;
    // Upload to Azure Blob (or local storage)
    var blobConn = config["AzureBlobConnectionString"];
    if (!string.IsNullOrWhiteSpace(blobConn))
    {
        var blobSvc = new BlobServiceClient(blobConn);
        var containerClient = blobSvc.GetBlobContainerClient("care-notes-pdfs");
        await containerClient.CreateIfNotExistsAsync();
        var blob = containerClient.GetBlobClient(fileName);
        await blob.UploadAsync(ms, overwrite: true);
        var url = blob.Uri.ToString();
        note.PdfUrl = url;
    }
    else
    {
        // For local dev: write to /data/pdfs
        Directory.CreateDirectory("pdfs");
        await File.WriteAllBytesAsync(Path.Combine("pdfs", fileName), ms.ToArray());
        var url = $"file://{Path.GetFullPath(Path.Combine("pdfs", fileName))}";
        note.PdfUrl = url;
    }

    await notes.ReplaceOneAsync(n => n.Id == note.Id, note);
    ch.BasicAck(ea.DeliveryTag, false);
};

ch.BasicConsume("note_pdf_queue", autoAck: false, consumer);

Console.WriteLine("PDF Worker running. Press [enter] to exit.");
Console.ReadLine();
