using DBContextSample.API.Services;
using DBContextSample.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DBContextSample.API.Test
{
    public class Test
    {
        private HttpClient _client;
        private CoreContext _context;
        private IFakeService _fakeService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var application = 
                new InMemoryApplicationFactory("DBContextSample_API_Test");
                //new ApplicationFactory();

            var scope = application.Services.CreateScope();

            _client = application.CreateClient();

            _context = scope.ServiceProvider.GetRequiredService<CoreContext>();
            _fakeService = scope.ServiceProvider.GetRequiredService<IFakeService>();
        }

        [SetUp]
        public async Task SetUp()
        {
            _context.People.Add(
                new Entities.Entities.Person
                {
                    FirstName = "Killian",
                    LastName = "Charlez"
                });

            await _context.SaveChangesAsync();
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

            response.EnsureSuccessStatusCode();

            var people = await response.Content.ReadAsStringAsync();
        }

        [Test]
        public async Task Http_Test2()
        {
            var response = await _client.GetAsync("/api/health");

            response.EnsureSuccessStatusCode();
        }
    }
}