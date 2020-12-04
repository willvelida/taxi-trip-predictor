using Azure.Storage.Blobs;
using System;
using System.Threading.Tasks;

namespace TaxiTripPredictor.ModelTrainer.Helpers
{
    public class BlobStorageHelpers : IBlobStorageHelpers
    {
        public BlobClient GetBlobClient(string connectionString, string blobContainerName, string blobName)
        {
            return new BlobClient(connectionString, blobContainerName, blobName);
        }

        public async Task UploadBlob(BlobClient blobClient, string blobName)
        {
            try
            {
                await blobClient.UploadAsync(blobName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not upload blob to blob storage. Exception thrown: {ex.Message}");
            }
        }
    }
}
