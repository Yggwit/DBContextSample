using DBContextSample.Context.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.Common;

namespace DBContextSample.Context
{
    public static class ContextTransactionExtention
    {
        public static DbTransaction DbTransaction<T>(this T context)
            where T : ITransactionContext
            => context.Transaction is not null
                ? (context.Transaction as IInfrastructure<DbTransaction>).Instance
                : null;


        public static T BeginTransaction<T>(this T context)
            where T : DbContext, ITransactionContext
        {
            if (context is ITrackedPropertiesContext trackedPropertiesContext)
                trackedPropertiesContext.TrackedPropertyValues = new();

            context.Transaction = context.Database.BeginTransaction();
            return context;
        }

        public static T BeginTransaction<T>(this T context, System.Data.IsolationLevel isolationLevel)
            where T : DbContext, ITransactionContext
        {
            if (context is ITrackedPropertiesContext trackedPropertiesContext)
                trackedPropertiesContext.TrackedPropertyValues = new();

            context.Transaction = context.Database.BeginTransaction(isolationLevel);
            return context;
        }

        public static T RollbackTransaction<T>(this T context)
            where T : DbContext, ITransactionContext
        {
            context.ChangeTracker.Clear();
            context.Transaction?.Rollback();

            return context;
        }

        public static T CommitTransaction<T>(this T context)
            where T : DbContext, ITransactionContext
        {
            context.SaveChanges();
            context.Transaction?.Commit();

            return context;
        }

        public static T DisposeTransaction<T>(this T context)
            where T : DbContext, ITransactionContext
        {
            context.Transaction?.Dispose();

            return context;
        }
    }
}
