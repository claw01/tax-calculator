using System.IO;
using System.Collections.Generic;
using TaxCalculator.Providers;
using TaxCalculator.Utilities;
using TaxCalculator.Dtos;
using TaxCalculator.Models;

namespace TaxCalculator.Services
{
    public class CalculateTaxService : ICalculateTaxService
    {
        private readonly ITaxBandProvider _taxBandProvider;
        private readonly ITaxBandHelper _taxBandHelper;
        public IReadOnlyList<TaxBand> TaxBands { get; private set; }

        public CalculateTaxService(ITaxBandProvider taxBandProvider, ITaxBandHelper taxBandHelper)
        {
            _taxBandProvider = taxBandProvider;
            _taxBandHelper = taxBandHelper;

            var taxBands = _taxBandProvider.GetAll();

            if (taxBandHelper.AnyDuplicatedUpperBound(taxBands))
            {
                throw new InvalidDataException("UpperBound value(s) are duplicated");
            };

            if (!taxBandHelper.AnyTailBand(taxBands))
            {
                throw new InvalidDataException("There should be one TaxBand with no UpperBound defined (Tail TaxBand)");
            }

            TaxBands = taxBandHelper.Sort(taxBands);
        }

        public (decimal,List<TaxDetailDto>) GetTax(decimal amount)
        {
            var lowerBound = 0m;
            var total = 0m;
            var details = new List<TaxDetailDto>();

            foreach (var taxBand in TaxBands)
            {
                total += taxBand.GetTax(lowerBound, amount);
                details.Add(TaxDetailDto.Create(lowerBound,
                    taxBand.UpperBound,
                    taxBand.Rate,
                    taxBand.Description,
                    taxBand.GetTax(lowerBound, amount)));
                lowerBound = taxBand.UpperBound.HasValue ? taxBand.UpperBound.Value : 0;
            }

            return (total, details);
        }


    }
}