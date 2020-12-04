using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using TaxiTripPredictor.ModelTrainer.Helpers;

namespace TaxiTripPredictor.ModelTrainer.Functions
{
    public class TrainAndUploadModel
    {
        private readonly ILogger<TrainAndUploadModel> _logger;
        private readonly IConfiguration _config;
        private readonly MLContext _mlContext;
        private readonly IBlobStorageHelpers _blobStorageHelpers;
        private readonly IModelTrainerHelpers _modelTrainerHelpers;

        public TrainAndUploadModel(
            ILogger<TrainAndUploadModel> logger,
            IConfiguration config,
            MLContext mlContext,
            IBlobStorageHelpers blobStorageHelpers,
            IModelTrainerHelpers modelTrainerHelpers)
        {
            _logger = logger;
            _config = config;
            _mlContext = mlContext;
            _blobStorageHelpers = blobStorageHelpers;
            _modelTrainerHelpers = modelTrainerHelpers;
        }

        [FunctionName(nameof(TrainAndUploadModel))]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer)
        {
            _logger.LogInformation("Starting model traininer");

            try
            {
                // Authenticate to Azure Storage
                BlobClient blobClient = _blobStorageHelpers.GetBlobClient(
                    _config["StorageConnectionString"],
                    _config["BlobContainerName"],
                    _config["BlobName"]);

                // Read file from local file soruce
                string trainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-train.csv");
                string testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-test.csv");
                string modelPath = _config["ModelPath"];

                // Train and Save Model
                await _modelTrainerHelpers.TrainAndSaveModel(
                    _mlContext,
                    trainDataPath,
                    testDataPath,
                    modelPath,
                    blobClient);
                _logger.LogInformation($"Model has been saved to {_config["BlobContainerName"]} named: {_config["ModelPath"]}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
            }
        }
    }
}
