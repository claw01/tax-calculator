using System.Collections.Generic;
using TaxCalculator.Dtos;
namespace TaxCalculator.Services
{
    public interface ICalculateTaxService
    {
        /// <summary>
        /// This should return the total tax combined of every Tax Band
        ///</summary>
        (decimal, List<TaxDetailDto>) GetTax(decimal amount);

    }
}