using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TaxCalculator.Providers;
using TaxCalculator.Services;
using TaxCalculator.Utilities;
using Newtonsoft.Json;

namespace TaxCalculator
{

    public static class Bootstraper
    {
        public static ServiceProvider Setup()
        {
            JsonConvert.DefaultSettings = () => { 
                var settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.Formatting = Formatting.Indented;
                return settings;
                };
            
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>((s) => LoadConfiguration());
            serviceCollection.AddSingleton<ITaxBandHelper, TaxBandHelper>();
            serviceCollection.AddSingleton<ICalculateTaxService, CalculateTaxService>();
            serviceCollection.AddSingleton<ITaxBandProvider, MongoDBTaxBandProvider>();

            return serviceCollection.BuildServiceProvider();
        }

        private static IConfiguration LoadConfiguration()
        {
            return new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
        }
    }
}