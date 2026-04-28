using System.Collections.Generic;
using System.Threading.Tasks;
using WinFIM.Core.Models;

namespace WinFIM.Core.Services
{
    public interface IExportService
    {
        Task ExportEventsToCsvAsync(string filePath, IEnumerable<FileEvent> events);
    }
}
