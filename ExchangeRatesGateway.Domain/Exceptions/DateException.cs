using System;

namespace ExchangeRateGateway.Domain.Exceptions
{
    public class DateException: ArgumentException
    {
        public DateException(string message, string parameterName) : base(message, parameterName) { }
    }
}