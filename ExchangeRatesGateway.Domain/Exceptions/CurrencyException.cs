using System;

namespace ExchangeRatesGateway.Domain.Exceptions
{
    public class CurrencyException : ArgumentException
    {
        public CurrencyException(string message, string parameterName) : base(message, parameterName) { }
    }
}