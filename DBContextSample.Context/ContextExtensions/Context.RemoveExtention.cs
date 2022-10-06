using DBContextSample.Entities.Entities;

namespace DBContextSample.Context
{
    public static class ContextRemoveExtention
    {
        public static void Remove<T>(this DbSet<T> entities, System.Linq.Expressions.Expression<Func<T, bool>> predicate)
            where T : class, IEntityBase
            => entities.RemoveRange(
                entities
                    .Where(predicate)
                    .ToList()
            );

        public static async Task RemoveAsync<T>(this DbSet<T> entities, System.Linq.Expressions.Expression<Func<T, bool>> predicate)
            where T : class, IEntityBase
            => entities.RemoveRange(
                await entities
                    .Where(predicate)
                    .ToListAsync()
            );
    }
}
