using System;

namespace WinFIM.Core.Models
{
    public class FileEvent
    {
        public int Id { get; set; }
        public int DirectoryId { get; set; }
        
        public string EventType { get; set; } = string.Empty; // e.g., CREATED, MODIFIED, DELETED, RENAMED
        public string Severity { get; set; } = "Low"; // e.g., Low, Medium, High
        public string FullPath { get; set; } = string.Empty;
        public string? OldPath { get; set; } // Only for RENAMED
        public DateTime DetectedAt { get; set; } = DateTime.Now;
        public string DetectionSource { get; set; } = string.Empty; // e.g., "Watcher", "Revalidation"
        public string? Details { get; set; }

        // Navigation property
        public virtual MonitoredDirectory Directory { get; set; } = null!;
    }
}
