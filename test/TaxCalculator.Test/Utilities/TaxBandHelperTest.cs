using Xunit;
using FluentAssertions;
using TaxCalculator.Models;
using System.Collections.Generic;

namespace TaxCalculator.Utilities.Tests
{
    public class TaxBandHelperTest
    {
        public static IEnumerable<object[]> Sort_ReturnTaxBandSortedByUpperBound_Data
        {
            get
            {
                var band1 = TaxBand.Create(20, "band1", 100);
                var band2 = TaxBand.Create(20, "band2", 200);
                var band3 = TaxBand.Create(20, "band3");

                var expectedResult = new List<TaxBand> { band1, band2, band3 };

                return new List<object[]>
                {
                    new object[] {new List<TaxBand> {band1, band2, band3},expectedResult},
                    new object[] {new List<TaxBand> {band1, band3, band2},expectedResult},
                    new object[] {new List<TaxBand> {band2, band3, band1},expectedResult},
                    new object[] {new List<TaxBand> {band2, band1, band3},expectedResult},
                    new object[] {new List<TaxBand> {band3, band2, band1},expectedResult},
                    new object[] {new List<TaxBand> {band3, band1, band2},expectedResult},
                };
            }
        }




        [Theory]
        [MemberData(nameof(Sort_ReturnTaxBandSortedByUpperBound_Data))]
        public void Sort_ReturnTaxBandSortedByUpperBound(List<TaxBand> taxBands, List<TaxBand> expectedResult)
        {
            var helper = new TaxBandHelper();
            var sortedTaxBands = helper.Sort(taxBands);

            sortedTaxBands.Should().HaveCount(3);
            sortedTaxBands.Should().Equal(expectedResult);
        }

        public static IEnumerable<object[]> AnyDuplicatedUpperBound_ReutrnTrueWhenUpperBoundHasSameValue_Data
        {
            get
            {
                var band1 = TaxBand.Create(20, "band1", 100);
                var band2 = TaxBand.Create(20, "band2", 200);
                var band3 = TaxBand.Create(20, "band3");
                var band2Duplicated = TaxBand.Create(20, "band2", 200);
                var band3Duplicated = TaxBand.Create(20, "band3");

                return new List<object[]>
                {
                    new object[] {new List<TaxBand> {band1, band2, band2Duplicated},true}, //should work on duplicate values
                    new object[] {new List<TaxBand> {band2Duplicated, band2, band3},true}, //should work on other order
                    new object[] {new List<TaxBand> {band1, band2, band3},false},
                    new object[] {new List<TaxBand> {band2, band3Duplicated, band3},true}, //should work even uppderbound is null
                };
            }
        }

        [Theory]
        [MemberData(nameof(AnyDuplicatedUpperBound_ReutrnTrueWhenUpperBoundHasSameValue_Data))]
        public void AnyDuplicatedUpperBound_ReutrnTrueWhenUpperBoundHasSameValue(List<TaxBand> taxBands, bool expectedResult)
        {
            var helper = new TaxBandHelper();
            var anyDuplicatedUpperBound = helper.AnyDuplicatedUpperBound(taxBands);

            anyDuplicatedUpperBound.Should().Be(expectedResult);
        }

        public static IEnumerable<object[]> AnyTailBand_ReutrnTrueWhenBandWithNullUpperBondPresent_Data
        {
            get
            {
                var band1 = TaxBand.Create(20, "band1", 100);
                var band2 = TaxBand.Create(20, "band2", 200);
                var band3 = TaxBand.Create(20, "band3", 300);
                var band4 = TaxBand.Create(20, "band4");

                return new List<object[]>
                {
                    new object[] {new List<TaxBand> {band1, band2, band3},false},
                    new object[] {new List<TaxBand> {band3, band2, band1},false},
                    new object[] {new List<TaxBand> {band1, band2, band4},true},
                    new object[] {new List<TaxBand> {band4, band2, band1},true},
                };
            }
        }

        [Theory]
        [MemberData(nameof(AnyTailBand_ReutrnTrueWhenBandWithNullUpperBondPresent_Data))]
        public void AnyTailBand_ReutrnTrueWhenBandWithNullUpperBondPresent(List<TaxBand> taxBands, bool expectedResult)
        {
            var helper = new TaxBandHelper();
            var anyDuplicatedUpperBound = helper.AnyTailBand(taxBands);

            anyDuplicatedUpperBound.Should().Be(expectedResult);
        }
    }
}