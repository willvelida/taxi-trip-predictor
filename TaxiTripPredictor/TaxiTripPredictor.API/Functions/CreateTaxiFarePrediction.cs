using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.ML;
using TaxiTripPredictor.Core.Models;
using TaxiTripPredictor.API.Models;
using TaxiTripPredictor.API.Repositories;

namespace TaxiTripPredictor.API.Functions
{
    public class CreateTaxiFarePrediction
    {
        private readonly ILogger<CreateTaxiFarePrediction> _logger;
        private readonly PredictionEnginePool<TaxiTrip, TaxiTripFarePrediction> _predictionEnginePool;
        private readonly ITaxiRepository _taxiRepository;

        public CreateTaxiFarePrediction(
            ILogger<CreateTaxiFarePrediction> logger,
            PredictionEnginePool<TaxiTrip, TaxiTripFarePrediction> predictionEnginePool,
            ITaxiRepository taxiRepository)
        {
            _logger = logger;
            _predictionEnginePool = predictionEnginePool;
            _taxiRepository = taxiRepository;
        }

        [FunctionName(nameof(CreateTaxiFarePrediction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "TaxiFare")] HttpRequest req)
        {
            IActionResult result = null;

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var input = JsonConvert.DeserializeObject<TaxiTrip>(requestBody);

                TaxiTripFarePrediction prediction = _predictionEnginePool.Predict(
                    modelName: "TaxiTripModel",
                    example: input);

                var taxiFarePrediction = new TaxiTripDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    VendorId = input.VendorId,
                    RateCode = input.RateCode,
                    PassengerCount = input.PassengerCount,
                    TripTime = input.TripTime,
                    TripDistance = input.TripDistance,
                    PaymentType = input.PaymentType,
                    FareAmount = input.FareAmount,
                    PredictedFareAmount = prediction.FareAmount
                };

                var response = await _taxiRepository.CreateTaxiPrediction(taxiFarePrediction);

                result = new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
