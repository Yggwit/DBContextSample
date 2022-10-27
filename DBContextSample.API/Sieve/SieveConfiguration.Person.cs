using DBContextSample.Entities.Entities;
using Sieve.Services;

namespace DBContextSample.API.Sieve
{
    public class SieveConfigurationPerson : ISieveConfiguration
    {
        public void Configure(SievePropertyMapper mapper)
        {
            mapper.Property<VwPerson>(p => p.FirstName)
                .CanSort()
                .CanFilter();
            mapper.Property<VwPerson>(p => p.LastName)
                .CanSort()
                .CanFilter();
        }
    }
}
