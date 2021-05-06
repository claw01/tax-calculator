using System;

namespace TaxCalculator.Models
{
    public class TaxBand : IComparable
    {
        public TaxBand(decimal rate, string description, decimal? upperBound = null)
        {
            if (rate < 0)
            {
                throw new ArgumentOutOfRangeException("rate must be a positive number");
            }

            if (upperBound < 0)
            {
                throw new ArgumentOutOfRangeException("upperBound must be a positive number");
            }

            if (String.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("description must not be null or empty");
            }

            Rate = rate;
            Description = description;
            UpperBound = upperBound;
        }
        public decimal? UpperBound { get; private set; }
        public decimal Rate { get; private set; }
        public string Description { get; private set; }

        /// <summary>
        /// GetTax of the income/amount of this taxBand
        /// lowerBound  
        /// </summary>
        /// <param name="lowerBound">The lowerBound of the previous band, should be 0 for the head band.</param>
        /// <param name="amount">The income amount .</param>
        public decimal GetTax(decimal lowerBound, decimal amount)
        {
            if (amount < 0 || lowerBound < 0)
            {
                throw new InvalidOperationException("amount/ lowerBound must be a postive number");
            }

            if (lowerBound > amount)
            {
                //if lowerBound larger than amount, nothing to calculate
                return 0;
            }

            if (amount >= UpperBound)
            {
                return (UpperBound.Value - lowerBound) * GetMultiplier();
            }

            return (amount - lowerBound) * GetMultiplier();
        }

        private decimal GetMultiplier()
        {
            return Rate / 100m;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("Taxband cannot be null");
            }

            var otherTaxBand = obj as TaxBand;
            if (otherTaxBand != null)
            {
                if (this.UpperBound.HasValue && otherTaxBand.UpperBound.HasValue)
                {
                    return this.UpperBound.Value.CompareTo(otherTaxBand.UpperBound);
                }

                return -1 * Nullable.Compare(this.UpperBound, otherTaxBand.UpperBound);
            }
            else
            {
                throw new ArgumentException("Object is not a TaxBand");
            }
        }

    }

}