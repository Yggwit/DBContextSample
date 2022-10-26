using DBContextSample.Entities.Entities;
using Sieve.Models;

namespace DBContextSample.API.Sieve
{
    public class FilterModel<T> : SieveModel
        where T : class, IEntityBase
    {
        private string fields = default!;

        public string Fields
        {
            get => fields;
            // Filter incoming fields; only <T> properties can be selected
            set => fields = string.IsNullOrEmpty(value)
                ? default!
                : string.Join(
                    ',',
                    value
                        .Split(",")
                        .Select(e => e.Trim())
                        .Intersect(
                            typeof(T).GetProperties().Select(p => p.Name),
                            StringComparer.OrdinalIgnoreCase
                        )
                );
        }
    }
}
