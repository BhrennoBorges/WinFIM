using System;
using System.Collections.Generic;

namespace WinFIM.Core.Models
{
    public class MonitoredDirectory
    {
        public int Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<BaselineFile> BaselineFiles { get; set; } = new List<BaselineFile>();
        public virtual ICollection<FileEvent> FileEvents { get; set; } = new List<FileEvent>();
    }
}
