using System;

namespace ExchangeRateGateway.Domain.Model
{
    public class HistoryRatesRequest
    {
        public DateTime?[] Dates { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
    }
}