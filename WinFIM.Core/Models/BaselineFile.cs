using System;

namespace WinFIM.Core.Models
{
    public class BaselineFile
    {
        public int Id { get; set; }
        public int SnapshotId { get; set; }
        public int DirectoryId { get; set; }
        
        public string FullPath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string Sha256Hash { get; set; } = string.Empty;

        // Navigation properties
        public virtual BaselineSnapshot Snapshot { get; set; } = null!;
        public virtual MonitoredDirectory Directory { get; set; } = null!;
    }
}
