using Microsoft.EntityFrameworkCore;

namespace DBContextSample.Context.InMemory
{
    public class SampleContextInMemory : SampleContext
    {
        public SampleContextInMemory(DbContextOptions<SampleContextInMemory> options)
            : base(options)
        { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            base.OnConfiguring(optionsBuilder);
        }
    }
}