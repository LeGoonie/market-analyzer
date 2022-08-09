using System.Reflection;

namespace MarketAnalyzer.Scrapper.Extensions
{
    public static class PropertyExtensions
    {
        public static bool TrySetValue<T>(this PropertyInfo property, object obj, T value)
        {
            try
            {
                property.SetValue(obj, value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}