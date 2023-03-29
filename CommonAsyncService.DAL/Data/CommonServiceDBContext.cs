using CommonAsyncService.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CommonAsyncService.DAL.Data
{
    public class CommonServiceDBContext : DbContext
    {
        public DbSet<HealthCheck> HealthChecks => Set<HealthCheck>();
        public DbSet<ServicesInfoHistory> ServicesInfoHistory => Set<ServicesInfoHistory>();
        public DbSet<StoryMailProcessing> StoryMailProcessing => Set<StoryMailProcessing>();
        public DbSet<User> Users => Set<User>();


        public CommonServiceDBContext(DbContextOptions<CommonServiceDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}