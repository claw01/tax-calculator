using System;
using Xunit;
using FluentAssertions;
using TaxCalculator.Models;
using System.Collections.Generic;

namespace TaxCalculator.Tests.Models
{
    public class TaxBandTest
    {
        [Theory]
        [InlineData(8000)]
        [InlineData(10000)]
        public void GetTax_WhenAmountLessThanOrEqualToUpperBound_ReturnsProductOfRateAndDifferenceOfAmountAndLowerBound(decimal amount)
        {
            var taxBand = TaxBand.Create(20, "band1", 10000);
            var tax = taxBand.GetTax(2000, amount);
            tax.Should().Be((amount - 2000) * (20 / 100m));
        }

        [Fact]
        public void GetTax_WhenAmountLargerThanUpperBound_ReturnsProductOfRateAndDifferenceOfUpperBoundAndLowerBound()
        {
            var taxBand = TaxBand.Create(20, "band1", 10000);
            var tax = taxBand.GetTax(2000, 12000);
            tax.Should().Be((10000m - 2000) * (20 / 100m));
        }

        [Theory]
        [InlineData(9000, 8000)]
        [InlineData(8000, 8000)]
        public void GetTax_WhenLowerBoundLargerThanAmount_ReturnZero(decimal lowerBound, decimal amount)
        {
            var taxBand = TaxBand.Create(20, "band1", 10000);
            var tax = taxBand.GetTax(lowerBound, amount);
            tax.Should().Be(0);
        }

        [Fact]
        public void GetTax_WhenUpperBoundIsNull_ReturnsProductOfRateAndDifferenceOfAmountAndLowerBound()
        {
            var taxBand = TaxBand.Create(20, "band1");
            var tax = taxBand.GetTax(2000m, 10000m);
            tax.Should().Be((10000m - 2000) * (20 / 100m));
        }

        [Fact]
        public void GetTax_WhenAmountLessThanZero_ThrowError()
        {
            var taxBand = TaxBand.Create(20, "band1");
            taxBand.Invoking(x => x.GetTax(0, -1m)).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GetTax_WhenBothAmountAndLowerBoundAreZero_ReturnZero()
        {
            var taxBand = TaxBand.Create(20, "band1");
            var tax = taxBand.GetTax(0, 0);
            tax.Should().Be(0);
        }

        [Fact]
        public void GetTax_WhenLowerBoundLessThanZero_ThrowError()
        {
            var taxBand = TaxBand.Create(20, "band1");
            taxBand.Invoking(x => x.GetTax(-1m, 1m)).Should().Throw<InvalidOperationException>();
        }

        public static IEnumerable<object[]> CompareTo_Data =>
                new List<object[]>
                {
                    new object[] {1000m, 1000m, 0},
                    new object[] {1000m, 900m, 1},
                    new object[] {900m, 1000m, -1},
                    new object[] {null, null, 0},
                    new object[] {null, 1000m, 1},
                    new object[] {1000m, null, -1}
                };

        //The reason to use MemeberData is because InlineData do not allow setting constant 
        [Theory]
        [MemberData(nameof(CompareTo_Data))]
        public void CompareTo_Test(decimal? upperBound1, decimal? upperBound2, int expectedResult)
        {
            var taxBand1 = TaxBand.Create(20, "band1", upperBound1);
            var taxBand2 = TaxBand.Create(20, "band2", upperBound2);

            var result = taxBand1.CompareTo(taxBand2);

            result.Should().Be(expectedResult);

        }
    }
}