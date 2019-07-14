using System.Threading.Tasks;
using ExchangeRatesGateway.Domain.Model;

namespace ExchangeRatesGateway.Domain
{
    public interface IExchangeRatesManagement
    {
        Task<ExchangeRatesResponse> GetRatesForGivenPeriodsAsync(HistoryRatesRequest request);
    }
}