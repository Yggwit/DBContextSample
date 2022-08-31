using DBContextSample.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace DBContextSample.Context
{
    public partial class SampleContext : DbContext
    {
        public virtual DbSet<Person> People { get; set; }

        public SampleContext(DbContextOptions<SampleContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
