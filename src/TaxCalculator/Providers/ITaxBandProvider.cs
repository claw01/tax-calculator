using System.Collections.Generic;
using TaxCalculator.Models;

namespace TaxCalculator.Providers
{
    public interface ITaxBandProvider
    {
        List<TaxBand> GetAll();
    }
}