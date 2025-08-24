using Shared.Models;
using System.Text.RegularExpressions;

namespace NoteService.Services
{
    public class NoteParser : INoteParser
    {
        // Simple heuristic-based parser
        public ParsedBirp Parse(string raw)
        {
            var parsed = new ParsedBirp();
            if (string.IsNullOrWhiteSpace(raw)) return parsed;

            raw = raw.Replace("\r\n", "\n");
            // Look for headings like Behavior:, Intervention:, Response:, Plan:
            var sections = Regex.Split(raw, @"(?m)^\s*(Behavior|Intervention|Response|Plan)\s*:\s*", RegexOptions.IgnoreCase);
            // If headings are found, sections will have alternating parts: [pretext, heading1, content1, heading2, content2...]
            if (sections.Length > 1)
            {
                for (int i = 1; i < sections.Length; i += 2)
                {
                    var heading = sections[i].Trim().ToLower();
                    var content = sections[i + 1].Trim();
                    switch (heading)
                    {
                        case "behavior": parsed.Behavior = content; break;
                        case "intervention": parsed.Intervention = content; break;
                        case "response": parsed.Response = content; break;
                        case "plan": parsed.Plan = content; break;
                    }
                }
                return parsed;
            }

            // Fallback: split by double newlines into 4 parts
            var parts = raw.Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 1) parsed.Behavior = parts.ElementAtOrDefault(0) ?? "";
            if (parts.Length >= 2) parsed.Intervention = parts.ElementAtOrDefault(1) ?? "";
            if (parts.Length >= 3) parsed.Response = parts.ElementAtOrDefault(2) ?? "";
            if (parts.Length >= 4) parsed.Plan = parts.ElementAtOrDefault(3) ?? "";
            return parsed;
        }
    }
}
