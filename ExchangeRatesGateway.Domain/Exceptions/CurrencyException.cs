using System;

namespace ExchangeRateGateway.Domain.Exceptions
{
    public class CurrencyException : ArgumentException
    {
        public CurrencyException(string message, string parameterName) : base(message, parameterName) { }
    }
}