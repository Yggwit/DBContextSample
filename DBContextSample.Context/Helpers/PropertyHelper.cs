using System.Reflection;

namespace DBContextSample.Context.Helpers
{
    internal class PropertyHelper
    {
        internal static bool TrySetProperty(object obj, string property, object value)
        {
            try
            {
                PropertyInfo prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
                if (prop?.CanWrite ?? false)
                {
                    prop.SetValue(obj, value, null);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        internal static bool PropertyExists(object obj, string property)
        {
            try
            {
                PropertyInfo prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
                if (prop?.CanWrite ?? false)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
