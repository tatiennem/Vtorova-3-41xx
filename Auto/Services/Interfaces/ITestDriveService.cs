using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Dto;
using Domain.Entities;

namespace Auto.Services.Interfaces
{
    public interface ITestDriveService
    {
        Task<TestDrive> CreateAsync(Car car, Customer customer, DateTime slot, string notes);
        Task<IReadOnlyList<TestDriveScheduleItem>> GetUpcomingAsync(int days);
    }
}
