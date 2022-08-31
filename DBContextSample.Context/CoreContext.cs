namespace DBContextSample.Context
{
    public partial class CoreContext : SampleContext
    {
        public CoreContext(DbContextOptions<CoreContext> options)
            : base(options)
        { }
    }
}
