using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace CurrencyManager.Helpers
{
    public static class DateTimeValidator
    {
        public static string IsValidDate(this string dt)
        {
            try
            {
                return DateOnly.ParseExact(dt, "yyyy-MM-dd").ToString("yyyy-MM-dd");
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
