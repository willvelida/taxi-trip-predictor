using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using System;
using System.IO;
using TaxiTripPredictor.API;
using TaxiTripPredictor.API.Repositories;
using TaxiTripPredictor.Core.Models;

[assembly: FunctionsStartup(typeof(Startup))]
namespace TaxiTripPredictor.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);

            var cosmosClientOptions = new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Direct
            };

            builder.Services.AddSingleton((s) => new CosmosClient(config["CosmosDBConnectionString"], cosmosClientOptions));
            builder.Services.AddSingleton<ITaxiRepository, TaxiRepository>();

            builder.Services.AddPredictionEnginePool<TaxiTrip, TaxiTripFarePrediction>()
                .FromUri(
                    modelName: "TaxiTripModel",
                    uri: config["ModelURL"],
                    period: TimeSpan.FromMinutes(1));
        }
    }
}
