using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WinFIM.Core.Models;

namespace WinFIM.Core.Services
{
    public class ExportService : IExportService
    {
        public async Task ExportEventsToCsvAsync(string filePath, IEnumerable<FileEvent> events)
        {
            var csvBuilder = new StringBuilder();
            
            // Header
            csvBuilder.AppendLine("Id,DetectadoEm,TipoEvento,Severidade,Origem,Caminho,CaminhoAntigo,Detalhes");

            foreach (var ev in events)
            {
                var id = ev.Id.ToString();
                var detectedAt = ev.DetectedAt.ToString("yyyy-MM-dd HH:mm:ss");
                var type = EscapeCsv(ev.EventType);
                var severity = EscapeCsv(ev.Severity);
                var source = EscapeCsv(ev.DetectionSource);
                var path = EscapeCsv(ev.FullPath);
                var oldPath = EscapeCsv(ev.OldPath);
                var details = EscapeCsv(ev.Details);

                csvBuilder.AppendLine($"{id},{detectedAt},{type},{severity},{source},{path},{oldPath},{details}");
            }

            await File.WriteAllTextAsync(filePath, csvBuilder.ToString(), Encoding.UTF8);
        }

        private string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";

            // If value contains comma, quotes or newlines, we must enclose it in quotes and double the internal quotes.
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\r") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}
