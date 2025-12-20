using System;

namespace Auto.Services.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
        DateTime Today { get; }
    }
}
