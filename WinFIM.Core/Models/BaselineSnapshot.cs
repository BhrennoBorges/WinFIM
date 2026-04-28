using System;
using System.Collections.Generic;

namespace WinFIM.Core.Models
{
    public class BaselineSnapshot
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Completed";
        public string? Notes { get; set; }

        // Navigation property
        public virtual ICollection<BaselineFile> BaselineFiles { get; set; } = new List<BaselineFile>();
    }
}
