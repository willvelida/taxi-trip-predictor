using System.Threading.Tasks;
using TaxiTripPredictor.API.Models;

namespace TaxiTripPredictor.API.Repositories
{
    public interface ITaxiRepository
    {
        /// <summary>
        /// Inserts a new taxi fare prediction into the database
        /// </summary>
        /// <param name="taxiTripDTO"></param>
        /// <returns></returns>
        Task<TaxiTripDTO> CreateTaxiPrediction(TaxiTripDTO taxiTripDTO);
    }
}
