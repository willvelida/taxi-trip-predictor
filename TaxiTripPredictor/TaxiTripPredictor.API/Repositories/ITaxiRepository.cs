using System.Threading.Tasks;
using TaxiTripPredictor.API.Models;

namespace TaxiTripPredictor.API.Repositories
{
    public interface ITaxiRepository
    {
        Task<TaxiTripDTO> CreateTaxiPrediction(TaxiTripDTO taxiTripDTO);
    }
}
