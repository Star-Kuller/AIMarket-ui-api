using System;

namespace IAE.Microservice.Domain.Exceptions
{
    public class PeriodInvalidException : Exception
    {
        public PeriodInvalidException(DateTime startDate, DateTime? endDate, Exception ex)
           : base($"Period with start date \"{startDate}\" and end date \"{ endDate}\"  is invalid.", ex)
        {
        }
    }
}
