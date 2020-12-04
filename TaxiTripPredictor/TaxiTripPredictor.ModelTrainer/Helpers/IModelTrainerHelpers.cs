using Azure.Storage.Blobs;
using Microsoft.ML;
using System.Threading.Tasks;

namespace TaxiTripPredictor.ModelTrainer.Helpers
{
    public interface IModelTrainerHelpers
    {
        Task TrainAndSaveModel(MLContext mlContext, string trainFilePath, string testFilePath, string modelPath, BlobClient blobClient);
    }
}
