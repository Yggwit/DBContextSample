using Microsoft.EntityFrameworkCore.Design;

namespace DBContextSample.Context
{
    public class CoreContextFactory : IDesignTimeDbContextFactory<CoreContext>
    {
        private readonly DbContextOptions<CoreContext> _coreContextOptions;


        public CoreContextFactory(DbContextOptions<CoreContext> coreContextOptions = null)
            => _coreContextOptions = coreContextOptions;

        public CoreContext CoreContext
            => new(_coreContextOptions);


        /// <summary>
        /// Used by controller code generator
        /// Do not use !
        /// </summary>
        public CoreContextFactory()
            : this(new DbContextOptionsBuilder<CoreContext>().UseSqlServer("Empty").Options)
        { }

        /// <summary>
        /// Used by controller code generator
        /// Do not use !
        /// </summary>
        public CoreContext CreateDbContext(string[] args)
            => new(_coreContextOptions);
    }
}
