using DBContextSample.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace DBContextSample.Context
{
    public static class ContextRemoveExtention
    {
        public static void Remove<T>(this DbSet<T> entities, System.Linq.Expressions.Expression<Func<T, bool>> predicate)
            where T : EntityBase
            => entities.RemoveRange(
                entities
                    .Where(predicate)
                    .ToList()
            );

        public static async Task RemoveAsync<T>(this DbSet<T> entities, System.Linq.Expressions.Expression<Func<T, bool>> predicate)
            where T : EntityBase
            => entities.RemoveRange(
                await entities
                    .Where(predicate)
                    .ToListAsync()
            );
    }
}
