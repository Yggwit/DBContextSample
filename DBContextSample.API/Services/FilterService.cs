using DBContextSample.API.Sieve;
using DBContextSample.Entities.Entities;
using Sieve.Models;
using Sieve.Services;
using System.Linq.Dynamic.Core;

namespace DBContextSample.API.Services
{
    public class FilterService
    {
        private readonly CoreContext _context;
        private readonly ISieveProcessor _sieveProcessor;

        public FilterService(CoreContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<object>> Filter<T>(FilterModel<T>? filter = null)
            where T : class, IEntityBase
        {
            IQueryable<T> query = _context.Set<T>()
                .AsNoTracking();

            if (filter is SieveModel sieveModel)
                query = _sieveProcessor
                    .Apply(sieveModel, query);

            return string.IsNullOrEmpty(filter?.Fields)
                ? await query
                    .ToListAsync<object>()
                : await query
                    .Select($"new {{{filter.Fields}}}")
                    .ToDynamicListAsync();
        }
    }
}
