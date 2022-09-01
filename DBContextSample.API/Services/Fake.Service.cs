using DBContextSample.Entities.Entities;

namespace DBContextSample.API.Services
{
    public class FakeService : IFakeService
    {
        private readonly CoreContext _context;

        public FakeService(CoreContext context)
            => _context = context;


        public async Task<List<Person>> GetPeople()
            => await _context.People
                .AsNoTracking()
                .ToListAsync();
    }
}
