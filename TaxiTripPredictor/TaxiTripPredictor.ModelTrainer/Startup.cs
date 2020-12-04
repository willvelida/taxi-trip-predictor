using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TaxiTripPredictor.ModelTrainer;
using TaxiTripPredictor.ModelTrainer.Helpers;

[assembly: FunctionsStartup(typeof(Startup))]
namespace TaxiTripPredictor.ModelTrainer
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);

            builder.Services.AddSingleton(sp => new MLContext(seed: 0));
            builder.Services.AddSingleton<IBlobStorageHelpers, BlobStorageHelpers>();
            builder.Services.AddSingleton<IModelTrainerHelpers, ModelTrainerHelpers>();
        }
    }
}
