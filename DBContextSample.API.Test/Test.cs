using DBContextSample.API.Services;
using DBContextSample.Context;
using DBContextSample.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DBContextSample.API.Test
{
    public class Test
    {
        private HttpClient _client = default!;
        private CoreContext _context = default!;
        private IFakeService _fakeService = default!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            try
            {
                var application =
                //new InMemoryApplicationFactory("DBContextSample_API_Test");
                new ApplicationFactory();

                var scope = application.Services.CreateScope();

                _client = application.CreateClient();

                _context = scope.ServiceProvider.GetRequiredService<CoreContext>();
                _fakeService = scope.ServiceProvider.GetRequiredService<IFakeService>();
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.Null);
            }
        }

        [SetUp]
        public async Task SetUp()
        {
            try
            {
                _context.People.Add(
                    new Person
                    {
                        FirstName = "Killian",
                        LastName = "Charlez"
                    });

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.Null);
            }
        }


        [Test]
        public async Task DI_Test1()
        {
            var people = await _fakeService
                .GetPeople();

            Assert.That(people, Is.Not.Null);
        }

        [Test]
        public async Task DI_Test2()
        {
            var people = await _context.People
                .AsNoTracking()
                .ToListAsync();

            Assert.That(people, Is.Not.Null);
        }


        [Test]
        public async Task Http_Test1()
        {
            var response = await _client.GetAsync("/api/people");

            response.EnsureSuccessStatusCode();

            var people = await response.Content.ReadAsStringAsync();

            Assert.That(people, Is.Not.Null);
        }

        [Test]
        public async Task Http_Test2()
        {
            var response = await _client.GetAsync("/api/health");

            response.EnsureSuccessStatusCode();
        }


        [Test]
        public void AddWithDefaultValues()
        {
            try
            {
                _context.People.AddWithDefaultValues(new Person(), new object { });

                _context.SaveChanges();

                List<Person> p = _context.People.ToList();
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.Null);
            }
        }


        [Test]
        public async Task TemporalTest()
        {
            try
            {
                Person person = new()
                {
                    FirstName = "Killian",
                    LastName = "Charlez"
                };

                _context.People.Add(person);
                await _context.SaveChangesAsync();

                person.Guid = Guid.NewGuid();
                await _context.SaveChangesAsync();

                var people = await _context.People
                    .TemporalAll()
                    .Select(e => new
                    {
                        Person = e,
                        StartTime = EF.Property<DateTime>(e, "StartTime"),
                        EndTime = EF.Property<DateTime>(e, "EndTime")
                    })
                    .ToListAsync();

                Assert.That(people, Is.Not.Null);
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.Null);
            }
        }
    }

    public static class AddWithDefaultValuesExtension
    {
        public static void AddWithDefaultValues<T, U>(this DbSet<T> dbSet, T entity, U _)
            where T : class, IEntityBase
        {
            entity.Guid = Guid.NewGuid();

            // automapper U => T

            dbSet.Add(entity);
        }
    }
}
