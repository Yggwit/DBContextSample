using DBContextSample.Context.Helpers;
using DBContextSample.Context.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DBContextSample.Context
{
    public static class ContextTrackedEntitiesExtention
    {
        internal static void AddTrackedPropertyValues<T>(this T context, EntityEntry entry)
            where T : DbContext, ITrackedPropertiesContext
        {
            foreach (string trackedProperty in context.TrackedProperties)
            {
                if (PropertyHelper.PropertyExists(entry.Entity, trackedProperty))
                {
                    string value = entry.CurrentValues.GetValue<string>(trackedProperty);

                    if (context.TrackedPropertyValues.ContainsKey(trackedProperty))
                    {
                        if (!context.TrackedPropertyValues[trackedProperty].Contains(value))
                            context.TrackedPropertyValues[trackedProperty].Add(value);
                    }
                    else
                    {
                        context.TrackedPropertyValues.Add(trackedProperty, new List<string> { value });
                    }
                }
            }
        }

        public static List<string> GetTrackedPropertyValues<T>(this T context, string property)
            where T : DbContext, ITrackedPropertiesContext
            => context.TrackedPropertyValues.ContainsKey(property)
                ? context.TrackedPropertyValues[property]
                : new();
    }
}
