using DBContextSample.API.Models;
using Sieve.Models;

namespace DBContextSample.API.Sieve
{
    public class FilterModel<R> : SieveModel
        where R : class, IFilterResult
    {
        private string fields = default!;
        public string Fields
        {
            get => fields;
            set => fields = string.IsNullOrEmpty(value)
                // Specify fields from filterResult
                ? string.Join(
                    ',',
                    typeof(R).GetProperties().Select(p => p.Name)
                )
                // Filter incoming fields; only <T> properties can be selected
                : string.Join(
                    ',',
                    value
                        .Split(",")
                        .Select(e => e.Trim())
                        .Intersect(
                            typeof(R).GetProperties().Select(p => p.Name),
                            StringComparer.OrdinalIgnoreCase
                        )
                );
        }

        public bool IncludeTotalCount { get; set; } = false;
        public bool IncludePageCount { get; set; } = false;
    }

    public class FilterResult<R>
        where R : class, IFilterResult
    {
        public int? TotalCount { get; set; }
        public int? PageCount { get; set; }
        public List<R> Results { get; set; } = default!;
    }
}
