using DBContextSample.Entities.Entities;

namespace DBContextSample.Context
{
    public abstract partial class SampleContext : DbContext
    {
        public virtual DbSet<Person> People { get; set; }

        public SampleContext(DbContextOptions<SampleContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Person_history", "dbo");
                        ttb
                            .HasPeriodStart("StartTime")
                            .HasColumnName("StartTime");
                        ttb
                            .HasPeriodEnd("EndTime")
                            .HasColumnName("EndTime");
                    }
                ));
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
