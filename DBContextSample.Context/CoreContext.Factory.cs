using Microsoft.EntityFrameworkCore.Design;

namespace DBContextSample.Context
{
    public class CoreContextFactory : IDesignTimeDbContextFactory<CoreContext>
    {
        private readonly DbContextOptions<CoreContext> _coreContextOptions;

        /// <summary>
        /// Used by controller code generator
        /// </summary>
        public CoreContextFactory()
            : this(
                new DbContextOptionsBuilder<CoreContext>()
                    .UseSqlServer("Server=.\\DBCONTEXTSAMPLE;Database=Sample;Trusted_Connection=True;Encrypt=False;App=DBContextSample.Scaffold")
                    .Options
            )
        { }

        public CoreContextFactory(DbContextOptions<CoreContext> coreContextOptions = null)
            => _coreContextOptions = coreContextOptions;

        /// <summary>
        /// Used by controller code generator
        /// </summary>
        public CoreContext CreateDbContext(string[] args)
            => new(_coreContextOptions);

        public CoreContext CoreContext
            => new(_coreContextOptions);
    }
}
