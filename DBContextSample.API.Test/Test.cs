using DBContextSample.API.Services;
using DBContextSample.Context;
using Microsoft.Extensions.DependencyInjection;

namespace DBContextSample.Test
{
    public class Test
    {
        private HttpClient _client;
        private CoreContext _context;
        private IFakeService _fakeService;

        [SetUp]
        public void Setup()
        {
            var application = new ApplicationFactory();

            var scope = application.Services.CreateScope();

            _client = application.CreateClient();

            _context = scope.ServiceProvider.GetRequiredService<CoreContext>();
            _fakeService = scope.ServiceProvider.GetRequiredService<IFakeService>();
        }


        [Test]
        public async Task DI_Test1()
        {
            var people = await _fakeService
                .GetPeople();
        }

        [Test]
        public async Task DI_Test2()
        {
            var people = await _context.People
                .AsNoTracking()
                .ToListAsync();
        }


        [Test]
        public async Task Http_Test1()
        {
            var response = await _client.GetAsync("/api/people");

            var people = await response.Content.ReadAsStringAsync();
        }
    }
}