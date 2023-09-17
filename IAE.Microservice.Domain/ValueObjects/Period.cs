using IAE.Microservice.Common;
using System;
using System.Collections.Generic;
using IAE.Microservice.Domain.Exceptions;
using IAE.Microservice.Domain.Infrastructure;

namespace IAE.Microservice.Domain.ValueObjects
{
    public class Period : ValueObject
    {
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        //public bool IsCompleted => EndDate.HasValue && EndDate < SystemTime.DateTime.UtcNow;
        //public bool IsStarted => StartDate < SystemTime.DateTime.UtcNow;
        public bool IsCompleted => EndDate.HasValue && EndDate.Value.Date < SystemTime.DateTime.UtcNow.Date;
        public bool IsStarted => StartDate.Date < SystemTime.DateTime.UtcNow.Date;

        private Period(DateTime startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public static Period Create(DateTime startDate, DateTime? endDate)
        {
            return Create(startDate, endDate, TimeZoneInfo.Utc);
        }

        public Period ToTimezone(TimeZoneInfo timeZoneInfo)
        {
            /*var startDate = TimeZoneInfo.ConvertTimeFromUtc(StartDate, timeZoneInfo);
            var endDate = EndDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(EndDate.Value, timeZoneInfo) : (DateTime?)null;
            return new Period(startDate, endDate);*/
            var starDate = DateTime.SpecifyKind(StartDate, DateTimeKind.Unspecified);
            var endDate = EndDate == null ? (DateTime?) null : DateTime.SpecifyKind(EndDate.Value, DateTimeKind.Unspecified);
            return new Period(starDate, endDate);
        }

        public Period ConvertTimezone(TimeZoneInfo sourceTimezone, TimeZoneInfo destTimezone)
        {
            /*var startDate = ConvertTime(StartDate, sourceTimezone, destTimezone);
            var endDate = EndDate.HasValue ? ConvertTime(EndDate.Value, sourceTimezone, destTimezone) : (DateTime?)null;
            return new Period(startDate, endDate);*/
            return new Period(StartDate, EndDate);
        }

        public bool IncludedInto(Period period)
        {
            if (StartDate < period.StartDate)
                return false;

            if (period.EndDate.HasValue && EndDate > period.EndDate)
                return false;

            return true;
        }

        public Period WithEndDate(DateTime startDate, DateTime? endDate, TimeZoneInfo timeZoneInfo)
        {
            try
            {
                startDate = GetStartDate(startDate); // old version - remove this line.

                if (endDate.HasValue)
                {
                    //endDate = GetEndDate(endDate.Value, timeZoneInfo);
                    endDate = GetEndDate(endDate.Value);
                    
                    if (StartDate > endDate)
                        throw new InvalidOperationException("start date should be less than or equal end date");
                }

                return new Period(startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new PeriodInvalidException(startDate, endDate, ex);
            }
        }

        internal static Period Create(DateTime startDate, DateTime? endDate, TimeSpan utcOffset)
        {
            try
            {
                startDate = new DateTimeOffset(new DateTime(
                            startDate.Year,
                            startDate.Month,
                            startDate.Day), utcOffset)
                    .UtcDateTime;

                if (endDate.HasValue)
                {
                    endDate = new DateTimeOffset(
                            new DateTime(
                                endDate.Value.Year,
                                endDate.Value.Month,
                                endDate.Value.Day, 23, 59, 59, 999), utcOffset)
                        .UtcDateTime;

                    if (startDate > endDate)
                        throw new InvalidOperationException("start date should be less than or equal end date");
                }

                return new Period(startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new PeriodInvalidException(startDate, endDate, ex);
            }
        }

        public static Period Create(
            DateTime startDate, DateTime? endDate, TimeZoneInfo timeZoneInfo)
        {
            try
            {
                //startDate = GetStartDate(startDate, timeZoneInfo);
                startDate = GetStartDate(startDate);

                if (endDate.HasValue)
                {
                    //endDate = GetEndDate(endDate.Value, timeZoneInfo);
                    endDate = GetEndDate(endDate.Value);

                    if (startDate > endDate)
                        throw new InvalidOperationException("start date should be less than or equal end date");
                }

                return new Period(startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new PeriodInvalidException(startDate, endDate, ex);
            }
        }

        public override string ToString()
        {
            const string format = "yyyy-MM-ddTHH:mm:ss";

            var sDate = StartDate.ToString(format);
            var eDate = EndDate.HasValue ? EndDate.Value.ToString(format) : "∞";
            return $"{sDate} - {eDate}";
        }

        public string ToString(TimeZoneInfo timezone) => ToTimezone(timezone).ToString();

        /*private DateTime ConvertTime(
            DateTime utcDateTime, TimeZoneInfo sourceTimezone, TimeZoneInfo destTimezone)
        {
            // don't convert already passed datetimes
            if (utcDateTime <= SystemTime.DateTime.UtcNow) return utcDateTime;

            var dateTimeInSourceTz = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, sourceTimezone);

            return TimeZoneInfo.ConvertTimeToUtc(dateTimeInSourceTz, destTimezone);
        }

        private static DateTime GetStartDate(DateTime startDate, TimeZoneInfo timeZoneInfo)
        {
            var startDateUnspec = DateTime.SpecifyKind(new DateTime(
                startDate.Year,
                startDate.Month,
                startDate.Day), DateTimeKind.Unspecified);
            var currentDate = TimeZoneInfo.ConvertTimeFromUtc(SystemTime.DateTime.UtcNow, timeZoneInfo).Date;
            return startDateUnspec.Date == currentDate
                ? SystemTime.DateTime.UtcNow.AddMinutes(1)
                : TimeZoneInfo.ConvertTimeToUtc(startDateUnspec, timeZoneInfo);
        }

        private static DateTime GetEndDate(DateTime endDate, TimeZoneInfo timeZoneInfo)
        {
            var endDateUnspec = DateTime.SpecifyKind(new DateTime(
                                    endDate.Year,
                                    endDate.Month,
                                    endDate.Day, 23, 59, 59, 999), DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(endDateUnspec, timeZoneInfo);
        }*/
        
        private static DateTime GetStartDate(DateTime startDate)
        {
            var newStartDate = startDate.Date;
            return DateTime.SpecifyKind(newStartDate, DateTimeKind.Utc);
        }

        private static DateTime GetEndDate(DateTime endDate)
        {
            var newEndDate = endDate.Date.Add(new TimeSpan(23, 59, 59));
            return DateTime.SpecifyKind(newEndDate, DateTimeKind.Utc);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return StartDate;
            yield return EndDate;
        }
    }
}