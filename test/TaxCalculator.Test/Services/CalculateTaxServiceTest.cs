using System;
using System.IO;
using Xunit;
using Moq;
using FluentAssertions;
using TaxCalculator.Models;
using TaxCalculator.Services;
using TaxCalculator.Providers;
using TaxCalculator.Utilities;
using System.Collections.Generic;

namespace TaxCalculator.Tests.Services
{
    public class CalculateTaxServiceTest
    {
        private (Mock<ITaxBandProvider>, Mock<ITaxBandHelper>, List<TaxBand>) GetMocks()
        {
            return (new Mock<ITaxBandProvider>(), new Mock<ITaxBandHelper>(), new List<TaxBand>());
        }
        [Fact]
        public void Init_GetDataFromProvider()
        {
            (var taxBandProviderMock, var taxBandHelperMock, _) = GetMocks();

            taxBandHelperMock.Setup(x => x.AnyDuplicatedUpperBound(It.IsAny<List<TaxBand>>())).Returns(false);
            taxBandHelperMock.Setup(x => x.AnyTailBand(It.IsAny<List<TaxBand>>())).Returns(true);

            var service = new CalculateTaxService(taxBandProviderMock.Object, taxBandHelperMock.Object);

            taxBandProviderMock.Verify(x => x.GetAll(), Times.Once());
        }



        [Fact]
        public void Init_WhenAnyUpperBoundDuplicated_ThrowError()
        {
            (var taxBandProviderMock, var taxBandHelperMock, var taxBandsMock) = GetMocks();

            taxBandProviderMock.Setup(x => x.GetAll()).Returns(taxBandsMock);
            taxBandHelperMock.Setup(x => x.AnyDuplicatedUpperBound(It.Is<List<TaxBand>>(x => x == taxBandsMock))).Returns(true);
            taxBandHelperMock.Setup(x => x.AnyTailBand(It.Is<List<TaxBand>>(x => x == taxBandsMock))).Returns(true);

            Action action = () => new CalculateTaxService(taxBandProviderMock.Object, taxBandHelperMock.Object);

            action.Should().Throw<InvalidDataException>();
        }


        [Fact]
        public void WhenNoTailBand_ThrowError()
        {
            (var taxBandProviderMock, var taxBandHelperMock, var taxBandsMock) = GetMocks();

            taxBandProviderMock.Setup(x => x.GetAll()).Returns(taxBandsMock);
            taxBandHelperMock.Setup(x => x.AnyDuplicatedUpperBound(It.IsAny<List<TaxBand>>())).Returns(false);
            taxBandHelperMock.Setup(x => x.AnyTailBand(It.Is<List<TaxBand>>(x => x == taxBandsMock))).Returns(false);

            Action action = () => new CalculateTaxService(taxBandProviderMock.Object, taxBandHelperMock.Object);

            action.Should().Throw<InvalidDataException>();
        }

        [Fact]
        public void Init_SortDataFromRepositoryAndAssignToTaxBands()
        {
            (var taxBandProviderMock, var taxBandHelperMock, var taxBandsMock) = GetMocks();

            var sortedTaxBandsMock = new List<TaxBand>();

            taxBandProviderMock.Setup(x => x.GetAll()).Returns(taxBandsMock);
            taxBandHelperMock.Setup(x => x.AnyDuplicatedUpperBound(It.IsAny<List<TaxBand>>())).Returns(false);
            taxBandHelperMock.Setup(x => x.AnyTailBand(It.Is<List<TaxBand>>(x => x == taxBandsMock))).Returns(true);
            taxBandHelperMock.Setup(x => x.Sort(It.Is<List<TaxBand>>(x => x == taxBandsMock))).Returns(sortedTaxBandsMock);

            var service = new CalculateTaxService(taxBandProviderMock.Object, taxBandHelperMock.Object);

            service.TaxBands.Should().Equal(sortedTaxBandsMock);
        }

        private Mock<TaxBand> GetTaxBandMock(decimal? upperBound, decimal rate = 0, string description = "", decimal getTaxResult = 0)
        {
            var bandMock = new Mock<TaxBand>();
            bandMock.Setup(x => x.UpperBound).Returns(upperBound);
            bandMock.Setup(x => x.Rate).Returns(rate);
            bandMock.Setup(x => x.Description).Returns(description);
            bandMock.Setup(x => x.GetTax(It.IsAny<decimal>(), It.IsAny<decimal>())).Returns(getTaxResult);
            return bandMock;
        }

        [Fact]
        public void GetTotalTax_InvokeEachTaxBandWithLowerBoundAndAmount()
        {
            (var taxBandProviderMock, var taxBandHelperMock, var taxBandsMock) = GetMocks();

            var band1Mock = GetTaxBandMock(100m);
            var band2Mock = GetTaxBandMock(200m);
            var band3Mock = GetTaxBandMock(300m);
            var band4Mock = GetTaxBandMock(null);

            var sortedTaxBandsMock = new List<TaxBand>() { band1Mock.Object, band2Mock.Object, band3Mock.Object, band4Mock.Object };

            taxBandProviderMock.Setup(x => x.GetAll()).Returns(taxBandsMock);
            taxBandHelperMock.Setup(x => x.AnyDuplicatedUpperBound(It.IsAny<List<TaxBand>>())).Returns(false);
            taxBandHelperMock.Setup(x => x.AnyTailBand(It.Is<List<TaxBand>>(x => x == taxBandsMock))).Returns(true);
            taxBandHelperMock.Setup(x => x.Sort(It.Is<List<TaxBand>>(x => x == taxBandsMock))).Returns(sortedTaxBandsMock);

            var service = new CalculateTaxService(taxBandProviderMock.Object, taxBandHelperMock.Object);
            service.GetTax(50000);

            band1Mock.Verify(x => x.GetTax(0, 50000));
            band2Mock.Verify(x => x.GetTax(100m, 50000));
            band3Mock.Verify(x => x.GetTax(200m, 50000));
            band4Mock.Verify(x => x.GetTax(300m, 50000));
        }

        [Fact]
        public void GetTaxDetails_ReturnTaxDetailOfEachBand()
        {
            (var taxBandProviderMock, var taxBandHelperMock, var taxBandsMock) = GetMocks();

            var band1Mock = GetTaxBandMock(100m, 20, "band1", 4);
            var band2Mock = GetTaxBandMock(200m, 20, "band2", 30);
            var band3Mock = GetTaxBandMock(300m, 30, "band3", 200);
            var band4Mock = GetTaxBandMock(null, 40, "band4", 1000);

            var sortedTaxBandsMock = new List<TaxBand>() { band1Mock.Object, band2Mock.Object, band3Mock.Object, band4Mock.Object };

            taxBandProviderMock.Setup(x => x.GetAll()).Returns(taxBandsMock);
            taxBandHelperMock.Setup(x => x.AnyDuplicatedUpperBound(It.IsAny<List<TaxBand>>())).Returns(false);
            taxBandHelperMock.Setup(x => x.AnyTailBand(It.Is<List<TaxBand>>(x => x == taxBandsMock))).Returns(true);
            taxBandHelperMock.Setup(x => x.Sort(It.Is<List<TaxBand>>(x => x == taxBandsMock))).Returns(sortedTaxBandsMock);

            var service = new CalculateTaxService(taxBandProviderMock.Object, taxBandHelperMock.Object);
            (var total, var details) = service.GetTax(50000);

            total.Should().Be(1234);
            details.Should().HaveCount(4);
            details[0].Description.Should().Be("band1");
            details[0].Rate.Should().Be(20);
            details[0].Tax.Should().Be("4.00");

            details[1].Description.Should().Be("band2");
            details[1].Rate.Should().Be(20);
            details[1].Tax.Should().Be("30.00");

            details[2].Description.Should().Be("band3");
            details[2].Rate.Should().Be(30);
            details[2].Tax.Should().Be("200.00");

            details[3].Description.Should().Be("band4");
            details[3].Rate.Should().Be(40);
            details[3].Tax.Should().Be("1000.00");
        }

        [Fact]
        public void IntegrationTest_WithMockProvider()
        {
            (var taxBandProviderMock, _, _) = GetMocks();

            var band1 = TaxBand.Create(0, "band1", 12_500m);
            var band2 = TaxBand.Create(20, "band2", 50_000m);
            var band3 = TaxBand.Create(40, "band3", 150_000m);
            var band4 = TaxBand.Create(45, "band4");

            var taxBands = new List<TaxBand>() { band1, band2, band3, band4 };

            taxBandProviderMock.Setup(x => x.GetAll()).Returns(taxBands);

            var service = new CalculateTaxService(taxBandProviderMock.Object, new TaxBandHelper());
            (var tax, _) = service.GetTax(52_000m);

            tax.Should().Be(8_300m);
        }
    }
}