using System;
using Auto.Services.Interfaces;

namespace Auto.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime Today => DateTime.UtcNow.Date;
    }
}
