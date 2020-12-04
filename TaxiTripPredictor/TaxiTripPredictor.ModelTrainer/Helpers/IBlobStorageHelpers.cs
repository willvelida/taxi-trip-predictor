using Azure.Storage.Blobs;
using System.Threading.Tasks;

namespace TaxiTripPredictor.ModelTrainer.Helpers
{
    public interface IBlobStorageHelpers
    {
        // Get Blob Container
        BlobClient GetBlobClient(string connectionString, string blobContainerName, string blobName);
        // Upload to Blob Storage
        Task UploadBlob(BlobClient blobClient, string blobName); 
    }
}
