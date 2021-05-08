using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

namespace TaxCalculator.Tests
{
    public class FunctionTest
    {
        Function _function = new Function();
        private APIGatewayProxyRequest CreateRequest(string amount = null, string detail = null)
        {
            var request = new APIGatewayProxyRequest();
            if (amount != null)
            {
                request.PathParameters = new Dictionary<string, string>() { { "amount", amount } };
            }

            if (detail != null)
            {
                request.QueryStringParameters = new Dictionary<string, string>() { { "detail", detail } };
            }

            return request;
        }

        private APIGatewayProxyResponse CreateExpectedResponse(string body, int statusCode)
        {
            return new APIGatewayProxyResponse
            {
                Body = body,
                StatusCode = statusCode,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
        [Fact]
        public void GetTotal()
        {
            var request = CreateRequest("52000.00");
            var context = new TestLambdaContext();
            var body = "{\n  \"totalTax\": \"8300.00\"\n}";

            var expectedResponse = CreateExpectedResponse(body, 200);

            var response = _function.FunctionHandler(request, context);

            response.Body.Should().Be(expectedResponse.Body);
            response.Headers.Should().Equal(expectedResponse.Headers);
            response.StatusCode.Should().Be(expectedResponse.StatusCode);
        }

        [Fact]
        public void GetTotal_AmountNull_Return400()
        {
            var request = CreateRequest();
            var context = new TestLambdaContext();
            var body = "{\n  \"message\": \"Please provide amount\"\n}";

            var expectedResponse = CreateExpectedResponse(body, 400);

            var response = _function.FunctionHandler(request, context);

            response.Body.Should().Be(expectedResponse.Body);
            response.Headers.Should().Equal(expectedResponse.Headers);
            response.StatusCode.Should().Be(expectedResponse.StatusCode);
        }

        [Fact]
        public void GetTotal_InvalidAmount_RetrunsBadRequest()
        {
            var request = CreateRequest("invalid_amount");
            var context = new TestLambdaContext();
            var body = "{\n  \"message\": \"Amount must be a valid number\"\n}";

            var expectedResponse = CreateExpectedResponse(body, 400);

            var response = _function.FunctionHandler(request, context);

            response.Body.Should().Be(expectedResponse.Body);
            response.Headers.Should().Equal(expectedResponse.Headers);
            response.StatusCode.Should().Be(expectedResponse.StatusCode);
        }

        [Fact]
        public void GetDetails()
        {
            var request = CreateRequest("52000.00", "true");
            var context = new TestLambdaContext();
            var body = @"{
  ""totalTax"": ""8300.00"",
  ""details"": [
    {
      ""Range"": ""Over 0.00 and less than or equal to 12500.00"",
      ""Rate"": 0.0,
      ""Description"": ""Personal Allowance"",
      ""Tax"": ""0.00""
    },
    {
      ""Range"": ""Over 12500.00 and less than or equal to 50000.00"",
      ""Rate"": 20.0,
      ""Description"": ""Basic Rate"",
      ""Tax"": ""7500.00""
    },
    {
      ""Range"": ""Over 50000.00 and less than or equal to 150000.00"",
      ""Rate"": 40.0,
      ""Description"": ""Higher Rate"",
      ""Tax"": ""800.00""
    },
    {
      ""Range"": ""Over 150000.00"",
      ""Rate"": 45.0,
      ""Description"": ""Additional Rate"",
      ""Tax"": ""0.00""
    }
  ]
}";
            var expectedResponse = CreateExpectedResponse(body, 200);

            var response = _function.FunctionHandler(request, context);

            response.Body.Should().Be(expectedResponse.Body);
            response.Headers.Should().Equal(expectedResponse.Headers);
            response.StatusCode.Should().Be(expectedResponse.StatusCode);
        }

        [Fact]
        public void GetDetailFlag_QueryParamNull_ReturnFalse()
        {
            _function.GetDetailFlag(null).Should().Be(false);
        }

        [Fact]
        public void GetDetailFlag_QueryParamHasNoDetailFlag_ReturnFalse()
        {
            var queryParam = new Dictionary<string, string>();
            _function.GetDetailFlag(queryParam).Should().Be(false);
        }

        [Theory]
        [InlineData("", true)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("invalid_value", false)]
        public void GetDetailFlag_QueryParamHasDetailFlag(string detailValue, bool expectedResult)
        {
            var queryParam = new Dictionary<string, string>() { { "detail", detailValue } };
            _function.GetDetailFlag(queryParam).Should().Be(expectedResult);
        }
    }
}