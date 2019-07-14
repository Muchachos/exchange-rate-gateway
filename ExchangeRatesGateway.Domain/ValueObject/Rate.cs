using System;

namespace ExchangeRatesGateway.Domain.ValueObject
{
    public class Rate
    {
        public decimal Value { get; }
        public string Date { get; }

        internal Rate(decimal value, DateTime date)
        {
            Value = value;
            Date = date.ToString("yyyy-MM-dd");
        }
    }
}