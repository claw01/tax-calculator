using TaxCalculator.Utilities;
using System.Text;
namespace TaxCalculator.Dtos
{
    public class TaxDetailDto
    {
        public string Range { get; private set; }
        public decimal Rate { get; private set; }
        public string Description { get; private set; }
        public string Tax { get; private set; }

        public static TaxDetailDto Create(decimal lowerBound, decimal? upperBound, decimal rate, string description, decimal tax)
        {
            var sb = new StringBuilder();
            sb.Append($"Over { FormatHelper.ToMoney(lowerBound)}");

            if (upperBound.HasValue)
            {
                sb.Append($" and less than or equal to {FormatHelper.ToMoney(upperBound.Value)}");
            }

            return new TaxDetailDto()
            {
                Range = sb.ToString(),
                Rate = rate,
                Description = description,
                Tax = FormatHelper.ToMoney(tax)
            };
        }


    }
}