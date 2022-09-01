using DBContextSample.Entities.Entities;

namespace DBContextSample.API.Services
{
    public interface IFakeService
    {
        public Task<List<Person>> GetPeople();
    }
}
