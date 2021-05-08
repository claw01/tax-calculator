using System.Collections.Generic;
using TaxCalculator.Models;
namespace TaxCalculator.Utilities
{
    public interface ITaxBandHelper
    {
        List<TaxBand> Sort(List<TaxBand> taxBands);
        bool AnyDuplicatedUpperBound(List<TaxBand> taxBands);
        bool AnyTailBand(List<TaxBand> taxBands);
    }
}