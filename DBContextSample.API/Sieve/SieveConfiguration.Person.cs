using DBContextSample.Entities.Entities;
using Sieve.Services;

namespace DBContextSample.API.Sieve
{
    public class SieveConfigurationPerson : ISieveConfiguration
    {
        public void Configure(SievePropertyMapper mapper)
        {
            mapper.Property<Person>(p => p.FirstName)
                .CanSort()
                .CanFilter();
            mapper.Property<Person>(p => p.LastName)
                .CanSort()
                .CanFilter();
        }
    }
}
