using System;

namespace ExchangeRateGateway.Domain.Model
{
    public class HistoryRatesRequest
    {
        public HistoryRatesRequest() { }
        public HistoryRatesRequest(DateTime[] dates, string baseCurrency, string targetCurrency)
        {
            Dates = dates;
            BaseCurrency = baseCurrency;
            TargetCurrency = targetCurrency;
        }
        
        public DateTime[] Dates { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
    }
}