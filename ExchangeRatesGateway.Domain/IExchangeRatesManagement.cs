using System.Threading.Tasks;
using ExchangeRateGateway.Domain.Model;

namespace ExchangeRateGateway.Domain
{
    public interface IExchangeRatesManagement
    {
        Task<ExchangeRatesResponse> GetRatesForGivenPeriodsAsync(HistoryRatesRequest request);
    }
}