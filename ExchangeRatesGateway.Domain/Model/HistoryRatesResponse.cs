using ExchangeRatesGateway.Domain.ValueObject;

namespace ExchangeRatesGateway.Domain.Model
{
    public class ExchangeRatesResponse
    {
        public Rate MinimumRate { get; }
        public Rate MaximumRate { get; }
        public decimal AverageRate { get; }
        
        internal ExchangeRatesResponse(Rate minimumRate, Rate maximumRate, decimal averageRate)
        {
            MinimumRate = minimumRate;
            MaximumRate = maximumRate;
            AverageRate = averageRate;
        }
    }
}