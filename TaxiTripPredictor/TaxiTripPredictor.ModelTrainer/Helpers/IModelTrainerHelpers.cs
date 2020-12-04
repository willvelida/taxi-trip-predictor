using Azure.Storage.Blobs;
using Microsoft.ML;
using System.Threading.Tasks;

namespace TaxiTripPredictor.ModelTrainer.Helpers
{
    public interface IModelTrainerHelpers
    {
        /// <summary>
        /// Trains based on training and testing data and uploads the trained model to blob storage
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="trainFilePath"></param>
        /// <param name="testFilePath"></param>
        /// <param name="modelPath"></param>
        /// <param name="blobClient"></param>
        /// <returns></returns>
        Task TrainAndSaveModel(MLContext mlContext, string trainFilePath, string testFilePath, string modelPath, BlobClient blobClient);
    }
}
