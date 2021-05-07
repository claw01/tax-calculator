using System.Globalization;

namespace TaxCalculator.Utilities
{

    public class FormatHelper
    {
        public static string ToMoney(decimal value)
        {
            return value.ToString("F", CultureInfo.InvariantCulture);
        }
    }
}