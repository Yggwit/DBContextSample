using Microsoft.EntityFrameworkCore.Storage;

namespace DBContextSample.Context.Interfaces
{
    public interface ITransactionContext
    {
        internal IDbContextTransaction Transaction { get; set; }
    }
}
