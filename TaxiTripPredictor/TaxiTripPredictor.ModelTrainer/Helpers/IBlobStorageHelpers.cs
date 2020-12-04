using Azure.Storage.Blobs;
using System.Threading.Tasks;

namespace TaxiTripPredictor.ModelTrainer.Helpers
{
    public interface IBlobStorageHelpers
    {
       /// <summary>
       /// Retrieves the Blob Client
       /// </summary>
       /// <param name="connectionString"></param>
       /// <param name="blobContainerName"></param>
       /// <param name="blobName"></param>
       /// <returns></returns>
        BlobClient GetBlobClient(string connectionString, string blobContainerName, string blobName);

        /// <summary>
        /// Uploads specified blob to Blob Storage
        /// </summary>
        /// <param name="blobClient"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Task UploadBlob(BlobClient blobClient, string blobName); 
    }
}
