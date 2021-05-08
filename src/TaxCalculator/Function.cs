using System.Collections.Generic;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using System;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.Extensions.DependencyInjection;
using TaxCalculator.Services;
using TaxCalculator.Utilities;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TaxCalculator
{
    public class Function
    {
        private const string Path_Amount_Key = "amount";
        private const string Query_Detail_Key = "detail";
        private readonly ServiceProvider _serviceProvider;
        public Function()
        {
            _serviceProvider = Bootstraper.Setup();
        }
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
            try
            {
                var showDetail = GetDetailFlag(apigProxyEvent.QueryStringParameters);
                var service = _serviceProvider.GetService<ICalculateTaxService>();

                (var totalTax, var details) = service.GetTax(GetAmount(apigProxyEvent));

                var body = new
                {
                    totalTax = FormatHelper.ToMoney(totalTax),
                    details = showDetail ? details : null
                };

                return CreateResponse(body, 200);
            }
            catch (ArgumentException ex)
            {
                LambdaLogger.Log(ex.Message);
                return CreateErrorResponse(ex.Message, 400);
            }
            catch (Exception ex)
            {
                LambdaLogger.Log(ex.Message);
                return CreateErrorResponse("The server encounter an internal error or misconfiguration and was unable to process your request", 500);
            }
        }

        private APIGatewayProxyResponse CreateErrorResponse(string errorMessage, int statusCode)
        {
            var body = new { message = errorMessage };
            return CreateResponse(body, statusCode);
        }
        private APIGatewayProxyResponse CreateResponse(object body, int statusCode)
        {
            return new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(body),
                StatusCode = statusCode,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
        private decimal GetAmount(APIGatewayProxyRequest apigProxyEvent)
        {
            if (apigProxyEvent.PathParameters == null || !apigProxyEvent.PathParameters.ContainsKey(Path_Amount_Key))
            {
                throw new ArgumentException("Please provide amount");
            }

            if (!Decimal.TryParse(apigProxyEvent.PathParameters[Path_Amount_Key], out decimal amountValue))
            {
                throw new ArgumentException("Amount must be a valid number");
            }
            return amountValue;
        }

        public bool GetDetailFlag(IDictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey(Query_Detail_Key))
            {
                return false;
            }

            if (String.IsNullOrEmpty(parameters[Query_Detail_Key]))
            {
                return true;
            }

            if (Boolean.TryParse(parameters[Query_Detail_Key], out var result))
            {
                return result;
            };

            return false;
        }
    }
}
