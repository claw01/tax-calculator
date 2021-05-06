using System.Globalization;
using System.Text;
namespace TaxCalculator.Dtos
{
    public class TaxDetailDto
    {
        public string Range { get; private set; }
        public decimal Rate { get; private set; }
        public string Description { get; private set; }
        public decimal Tax { get; private set; }

        public static TaxDetailDto Create(decimal lowerBound, decimal? upperBound, decimal rate, string description, decimal tax)
        {
            var sb = new StringBuilder();
            sb.Append($"Over { FormatMoney(lowerBound)}");

            if (upperBound.HasValue)
            {
                sb.Append($" and less than or equal to {FormatMoney(upperBound.Value)}");
            }

            return new TaxDetailDto()
            {
                Range = sb.ToString(),
                Rate = rate,
                Description = description,
                Tax = tax
            };
        }

        public static string FormatMoney(decimal value)
        {
            return value.ToString("F", CultureInfo.InvariantCulture);
        }
    }
}