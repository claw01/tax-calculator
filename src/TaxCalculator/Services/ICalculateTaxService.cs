using System.Collections.Generic;
using TaxCalculator.Dtos;
namespace TaxCalculator.Models
{
    public interface ICalculateTaxService
    {
        /// <summary>
        /// This should return the total tax combined of every Tax Band
        ///</summary>
        decimal GetTotalTax(decimal amount);
        
        List<TaxDetailDto> GetTaxDetails(decimal amount);
    }
}