using System.Collections.Generic;
using WinFIM.Core.Models;

namespace WinFIM.Core.Services
{
    public interface IFileMonitorService
    {
        void StartMonitoring();
        void StopMonitoring();
        void ConfigureDirectories(IEnumerable<MonitoredDirectory> directories);
    }
}
