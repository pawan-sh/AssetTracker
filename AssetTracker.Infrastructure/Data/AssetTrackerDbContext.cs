using AssetTracker.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetTracker.Infrastructure.Data
{
    public class AssetTrackerDbContext : DbContext
    {
        public AssetTrackerDbContext(DbContextOptions<AssetTrackerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<AssetAssignment> AssetAssignments { get; set; }
    }
}
