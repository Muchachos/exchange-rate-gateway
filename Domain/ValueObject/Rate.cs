using System;

namespace ExchangeRateGateway.Domain.ValueObject
{
    public class Rate
    {
        public decimal Value { get; }
        public DateTime Date { get; }

        internal Rate(decimal value, DateTime date)
        {
            Value = value;
            Date = date;
        }
    }
}