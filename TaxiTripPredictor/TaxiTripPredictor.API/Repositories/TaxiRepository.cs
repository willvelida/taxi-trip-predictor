using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using TaxiTripPredictor.API.Models;

namespace TaxiTripPredictor.API.Repositories
{
    public class TaxiRepository : ITaxiRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IConfiguration _configuration;
        private readonly Container _taxiContainer;

        public TaxiRepository(
            CosmosClient cosmosClient,
            IConfiguration configuration)
        {
            _cosmosClient = cosmosClient;
            _configuration = configuration;

            _taxiContainer = _cosmosClient.GetContainer(_configuration["DatabaseName"], _configuration["ContainerName"]);
        }

        public async Task<TaxiTripDTO> CreateTaxiPrediction(TaxiTripDTO taxiTripDTO)
        {
            ItemResponse<TaxiTripDTO> taxiResponse = await _taxiContainer.CreateItemAsync(
                taxiTripDTO,
                new PartitionKey(taxiTripDTO.VendorId));

            return taxiResponse.Resource;
        }
    }
}
