using Microsoft.EntityFrameworkCore;
using WinFIM.Core.Interfaces;
using WinFIM.Core.Models;

namespace WinFIM.Data.Context
{
    public class WinFimDbContext : DbContext, IWinFimDbContext
    {
        public DbSet<MonitoredDirectory> MonitoredDirectories { get; set; } = null!;
        public DbSet<BaselineSnapshot> BaselineSnapshots { get; set; } = null!;
        public DbSet<BaselineFile> BaselineFiles { get; set; } = null!;
        public DbSet<FileEvent> FileEvents { get; set; } = null!;
        public DbSet<AppSetting> AppSettings { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // For a local desktop app, we will use a local sqlite file
            optionsBuilder.UseSqlite("Data Source=winfim.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure AppSetting Key as Primary Key
            modelBuilder.Entity<AppSetting>()
                .HasKey(a => a.Key);

            // Configure Relationships
            modelBuilder.Entity<BaselineFile>()
                .HasOne(b => b.Snapshot)
                .WithMany(s => s.BaselineFiles)
                .HasForeignKey(b => b.SnapshotId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BaselineFile>()
                .HasOne(b => b.Directory)
                .WithMany(d => d.BaselineFiles)
                .HasForeignKey(b => b.DirectoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FileEvent>()
                .HasOne(e => e.Directory)
                .WithMany(d => d.FileEvents)
                .HasForeignKey(e => e.DirectoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
