using TaxCalculator.Models;
using System.Collections.Generic;

namespace TaxCalculator.Repositories
{
    public interface ITaxBandProvider
    {
        List<TaxBand> GetAll();
    }
}