using Shared.Models;

namespace NoteService.Services
{
    public interface INoteParser
    {
        ParsedBirp Parse(string raw);
    }
}
