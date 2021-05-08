using System.Collections.Generic;
using System.Linq;
using TaxCalculator.Models;
namespace TaxCalculator.Utilities
{
    public class TaxBandHelper : ITaxBandHelper
    {
        public List<TaxBand> Sort(List<TaxBand> taxBands)
        {
            return taxBands.OrderBy(x => x).ToList();
        }

        public bool AnyDuplicatedUpperBound(List<TaxBand> taxBands)
        {
            return taxBands.Select(x => x.UpperBound).Distinct().Count() != taxBands.Count();
        }

        public bool AnyTailBand(List<TaxBand> taxBands)
        {
            return taxBands.Any(x => x.UpperBound == null);
        }
    }
}