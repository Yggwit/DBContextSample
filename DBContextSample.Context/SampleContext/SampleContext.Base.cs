using DBContextSample.Context.Helpers;
using DBContextSample.Context.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reflection;

namespace DBContextSample.Context
{
    public abstract partial class SampleContext : DbContext, ITransactionContext, ITrackedPropertiesContext
    {
        private static readonly ILogger _logger = DbContextLogger.CreateLogger<SampleContext>();

        protected SampleContext(DbContextOptions options) : base(options) { }


        #region CurrentUserID

        public int? CurrentUserID { get; protected set; } = null;

        #endregion

        #region Tracked properties

        public static List<string> TrackedProperties { get; set; } = new();
        private Dictionary<string, List<string>> TrackedPropertyValues { get; set; } = new();

        List<string> ITrackedPropertiesContext.TrackedProperties { get => TrackedProperties; set => TrackedProperties = value; }
        Dictionary<string, List<string>> ITrackedPropertiesContext.TrackedPropertyValues { get => TrackedPropertyValues; set => TrackedPropertyValues = value; }

        #endregion

        #region SaveChanges

        public override int SaveChanges()
        {
            int? currentUserID = CurrentUserID;

            try
            {
                foreach (
                    EntityEntry entry
                    in ChangeTracker.Entries()
                        .Where(e => e.State is EntityState.Added or EntityState.Modified)
                )
                {
                    // Generate synchro Guid
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            PropertyHelper.TrySetProperty(entry.Entity, "Guid", Guid.NewGuid().ToString());
                            break;
                        case EntityState.Modified:
                            if (
                                PropertyHelper.PropertyExists(entry.Entity, "Guid")
                                && entry.OriginalValues.GetValue<string>("Guid") != entry.CurrentValues.GetValue<string>("Guid")
                            )
                                //invalidate update on Guid field for update
                                Entry(entry.Entity).Property("Guid").IsModified = false;
                            break;
                    }

                    // Update BizzSync_%
                    PropertyHelper.TrySetProperty(entry.Entity, "BizzSync_TimeModif", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                    if (
                        PropertyHelper.TrySetProperty(entry.Entity, "BizzSync_UserID", currentUserID)
                        && entry.State == EntityState.Modified
                    )
                        Entry(entry.Entity).Property("BizzSync_UserID").IsModified = true;

                    // Track temporal table changes
                    PropertyHelper.TrySetProperty(entry.Entity, "VNEXT_Modified", DateTime.Now);
                    PropertyHelper.TrySetProperty(entry.Entity, "VNEXT_UserID", currentUserID);

                    this.AddTrackedPropertyValues(entry);
                }

                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                try
                {
                    List<object> entityErrors = new();
                    foreach (EntityEntry entry in ex.Entries)
                    {
                        PropertyValues proposedValues = entry.CurrentValues;
                        PropertyValues databaseValues = entry.GetDatabaseValues();

                        entityErrors.AddRange(
                            proposedValues.Properties
                                .Select(property => new
                                {
                                    Property = property,
                                    ProposedValue = proposedValues[property],
                                    DatabaseValue = databaseValues?[property]
                                })
                        );
                    }

                    _logger?.Error("{0}{1}Entries errors:{1}", ex, Environment.NewLine, entityErrors);
                }
                catch (Exception e)
                {
                    _logger?.Error(e, e.Message);
                }

                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger?.Error("{0}{1}Entries errors:{1}", ex, Environment.NewLine, ex.Entries.Select(e => string.Join(Environment.NewLine, e.Entity.GetType().Name)));
                throw;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, ex.Message);
                throw;
            }
        }

        #endregion

        #region Transaction

        private IDbContextTransaction _transaction = null;

        IDbContextTransaction ITransactionContext.Transaction { get => _transaction; set => _transaction = value; }

        #endregion

        #region Sql functions

        public int? DatePart(string datePartArg, DateTime? date)
            => throw new InvalidOperationException($"{nameof(DatePart)} cannot be called client side.");

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            MethodInfo datePartMethodInfo = typeof(SampleContext)
                .GetRuntimeMethod(nameof(SampleContext.DatePart), new[] { typeof(string), typeof(DateTime) });

            modelBuilder
                .HasDbFunction(datePartMethodInfo)
                .HasTranslation(args => new SqlFunctionExpression(
                        "DATEPART",
                        new[]
                        {
                            new SqlFragmentExpression((args.ToArray()[0] as SqlConstantExpression).Value.ToString()),
                            args.ToArray()[1]
                        },
                        true,
                        new[] { false, false },
                        typeof(int?),
                        null
                    )
                );
        }

        #endregion
    }
}
