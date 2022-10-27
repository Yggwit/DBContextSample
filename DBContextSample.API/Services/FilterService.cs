using DBContextSample.API.Models;
using DBContextSample.API.Sieve;
using DBContextSample.Entities.Entities;
using Sieve.Services;
using System.Linq.Dynamic.Core;

namespace DBContextSample.API.Services
{
    public class FilterService<C>
        where C : DbContext
    {
        private readonly DbContext _context;
        private readonly ISieveProcessor _sieveProcessor;

        public FilterService(C context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<FilterResult<R>> Filter<E, R>(FilterModel<R> filter)
            where E : class, IEntityBase
            where R : class, IFilterResult
        {
            IQueryable<E> query = _context.Set<E>()
                .AsNoTracking();
            int count = default;

            if (
                filter.IncludeTotalCount
                || (
                    filter.IncludePageCount
                    && (filter.PageSize ?? 0) > 0
                )
            )
                count = await _sieveProcessor
                   .Apply(filter, query, applyPagination: false, applySorting: false)
                   .CountAsync();

            query = _sieveProcessor
                .Apply(filter, query);

            List<R> result = await query
                .Select<R>($"new {{{filter.Fields}}}")
                .ToDynamicListAsync<R>();

            return new FilterResult<R>
            {
                TotalCount = filter.IncludeTotalCount
                    ? count
                    : null,
                PageCount = filter.IncludePageCount
                    ? (filter.PageSize ?? 0) > 0
                        ? (int)Math.Ceiling(count / (decimal)filter.PageSize!)
                        : 1
                    : null,

                Results = result
            };
        }
    }
}
