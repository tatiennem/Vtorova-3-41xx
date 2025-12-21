using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Dto;

namespace Auto.Services.Interfaces
{
    public interface IReportService
    {
        Task<IReadOnlyList<MonthlySalesReport>> GetMonthlySalesAsync(int months);
        Task<IReadOnlyList<ModelSalesReport>> GetModelSalesAsync();
        Task<IReadOnlyList<ModelMonthRevenue>> GetModelRevenueForCurrentMonthAsync();
        Task<IReadOnlyList<TestDriveScheduleItem>> GetUpcomingTestDrivesAsync(int days);

    }
}
