using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeRatesGateway.Domain.Model
{
    internal class ExternalRatesApiResponse
    {
        public Dictionary<string, decimal?> Rates { get; set; }

        public DateTime Date { get; set; }
        
        public decimal GetRate()
        {
            if (Rates == null || !Rates.Any())
                throw new Exception("Cannot fetch exchange rate.");

            var ratesOnDate = Rates.First().Value;
            if (!ratesOnDate.HasValue)
                throw new Exception("Cannot fetch exchange rate.");

            var rateToTargetCurrency = ratesOnDate.Value;
            return rateToTargetCurrency;
        }
    }
}
