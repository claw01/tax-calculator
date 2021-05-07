using TaxCalculator.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using System.Threading.Tasks;

namespace TaxCalculator.Providers
{
    public class MongoDBTaxBandProvider : ITaxBandProvider
    {
        private const string ConnectionString = "MONGO_DB_CONNECTION_STR";
        private const string DbName = "MONGO_DB_NAME";
        private readonly IConfiguration _configuration;
        private readonly MongoClient _client;
        public MongoDBTaxBandProvider(IConfiguration configuration)
        {
            CreateMapping();
            _configuration = configuration;
            _client = new MongoClient(_configuration[ConnectionString]);
        }

        public static void CreateMapping()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(TaxBand)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<TaxBand>(tb =>
            {
                tb.AutoMap();
                tb.SetIgnoreExtraElements(true);
            });
        }

        public List<TaxBand> GetAll()
        {
            var db = _client.GetDatabase(_configuration[DbName]);
            return db.GetCollection<TaxBand>("TaxBand").AsQueryable().ToList();
        }
    }

}