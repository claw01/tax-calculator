using System;
using System.IO;
using Xunit;
using Moq;
using FluentAssertions;
using TaxCalculator.Models;
using TaxCalculator.Repositories;
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
            service.GetTotalTax(50000);

            band1Mock.Verify(x => x.GetTax(0, 50000));
            band2Mock.Verify(x => x.GetTax(100m, 50000));
            band3Mock.Verify(x => x.GetTax(200m, 50000));
            band4Mock.Verify(x => x.GetTax(300m, 50000));
        }

        [Fact]
        public void GetTotalTax_ReturnSumOfAllTaxBandGetTaxResult()
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
            var tax = service.GetTotalTax(50000);

            tax.Should().Be(1234);
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
            var taxs = service.GetTaxDetails(50000);

            taxs.Should().HaveCount(4);
            taxs[0].Description.Should().Be("band1");
            taxs[0].Rate.Should().Be(20);
            taxs[0].Tax.Should().Be(4);

            taxs[1].Description.Should().Be("band2");
            taxs[1].Rate.Should().Be(20);
            taxs[1].Tax.Should().Be(30);

            taxs[2].Description.Should().Be("band3");
            taxs[2].Rate.Should().Be(30);
            taxs[2].Tax.Should().Be(200);

            taxs[3].Description.Should().Be("band4");
            taxs[3].Rate.Should().Be(40);
            taxs[3].Tax.Should().Be(1000);
        }

        [Fact]
        public void IntegrationTest_WithMockProvider(){ 
           (var taxBandProviderMock, _, _) = GetMocks();

            var band1 = new TaxBand(0, "band1", 12_500m);
            var band2 = new TaxBand(20, "band2", 50_000m);
            var band3 = new TaxBand(40, "band3", 150_000m);
            var band4 = new TaxBand(45, "band4");

            var taxBands = new List<TaxBand>() { band1,band2,band3,band4};

            taxBandProviderMock.Setup(x => x.GetAll()).Returns(taxBands);

            var service = new CalculateTaxService(taxBandProviderMock.Object, new TaxBandHelper());
            var tax = service.GetTotalTax(52_000m);

            tax.Should().Be(8_300m);
        }
    }
}