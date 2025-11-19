using AssetTracker.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;   // <-- Add this
using Microsoft.AspNetCore.Identity;                        // <-- Add this

namespace AssetTracker.Infrastructure.Data
{
    // IMPORTANT: Change DbContext → IdentityDbContext<IdentityUser>
    public class AssetTrackerDbContext : IdentityDbContext<IdentityUser>
    {
        public AssetTrackerDbContext(DbContextOptions<AssetTrackerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<AssetAssignment> AssetAssignments { get; set; }

        public DbSet<Repair> Repairs { get; set; }

        public DbSet<Issue> Issues { get; set; }

    }
}
