using Microsoft.EntityFrameworkCore;
using WerkWerk.Data;

namespace WerkWerk.Test.Model
{
    public class TestContext : DbContext
    {
        public DbSet<Job> Jobs { get; set; }

        public TestContext(DbContextOptions<TestContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>(Job.DefaultEntitySetup);
        }
    }
}