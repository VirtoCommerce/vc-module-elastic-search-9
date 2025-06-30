using System.Globalization;

namespace VirtoCommerce.ElasticSearch9.Data.Extensions
{
    public static class ElasticSearchNameExtensions
    {
        public static string ToElasticFieldName(this string originalName)
        {
            return originalName?.ToLowerInvariant();
        }

        public static string ToStringInvariant(this object value)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", value);
        }
    }
}
