using System;
using System.IO;
using Xunit;
using Moq;
using FluentAssertions;
using TaxCalculator.Dtos;
using TaxCalculator.Providers;
using TaxCalculator.Utilities;
using System.Collections.Generic;

namespace TaxCalculator.Tests.Services
{
    public class TaxDetailDtoTest
    {
        [Fact]
        public void Create_ShouldReturnDto()
        {
            var detail = TaxDetailDto.Create(10000, 20000, 20, "band1", 2000);

            detail.Description.Should().Be("band1");
            detail.Range.Should().Be("Over 10000.00 and less than or equal to 20000.00");
            detail.Tax.Should().Be("2000.00");
            detail.Rate.Should().Be(20);
        }

        [Fact]
        public void Create_UppderBoundNull_ShouldReturnRangeStatementWithOverOnly()
        {
            var detail = TaxDetailDto.Create(10000, null, 20, "band1", 2000);

            detail.Description.Should().Be("band1");
            detail.Range.Should().Be("Over 10000.00");
            detail.Tax.Should().Be("2000.00");
            detail.Rate.Should().Be(20);
        }
    }
}