using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.DbContext;
using Domain.Dto;
using Auto.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Auto.Services
{
    public class ReportService : IReportService
    {
        private readonly IDbContextFactory<AutoSalonContext> _factory;
        private readonly IDateTimeProvider _clock;

        public ReportService(IDbContextFactory<AutoSalonContext> factory, IDateTimeProvider clock)
        {
            _factory = factory;
            _clock = clock;
        }

        public async Task<IReadOnlyList<MonthlySalesReport>> GetMonthlySalesAsync(int months)
        {
            var from = _clock.Today.AddMonths(-(months - 1));

            await using var db = await _factory.CreateDbContextAsync();

            var raw = await db.Sales
                .Where(s => s.SaleDate >= from)
                .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Count = g.Count(),
                    Revenue = g.Sum(s => s.TotalPrice)
                })
                .OrderBy(r => r.Year).ThenBy(r => r.Month)
                .ToListAsync();

            return raw.Select(r => new MonthlySalesReport
            {
                Period = $"{r.Month:D2}.{r.Year}",
                CarsSold = r.Count,
                Revenue = r.Revenue
            }).ToList();
        }

        public async Task<IReadOnlyList<ModelSalesReport>> GetModelSalesAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();

            var raw = await db.Sales
                .Include(s => s.Car).ThenInclude(c => c.Model)
                .GroupBy(s => s.Car.Model.Name)
                .Select(g => new ModelSalesReport
                {
                    Model = g.Key,
                    Sold = g.Count(),
                    Revenue = g.Sum(s => s.TotalPrice)
                })
                .OrderByDescending(r => r.Sold)
                .ToListAsync();

            return raw;
        }

        public async Task<IReadOnlyList<TestDriveScheduleItem>> GetUpcomingTestDrivesAsync(int days)
        {
            var from = _clock.Today;
            var to = from.AddDays(days);

            await using var db = await _factory.CreateDbContextAsync();

            var raw = await db.TestDrives
                .Include(t => t.Car).ThenInclude(c => c.Model)
                .Include(t => t.Customer)
                .Where(t => t.Slot >= from && t.Slot <= to)
                .OrderBy(t => t.Slot)
                .ToListAsync();

            return raw.Select(t => new TestDriveScheduleItem
            {
                Slot = DateTime.SpecifyKind(t.Slot, DateTimeKind.Utc).ToLocalTime(),
                Car = $"{t.Car.Model.Name} {t.Car.Color}",
                Customer = t.Customer.FullName,
                Phone = t.Customer.Phone
            }).ToList();
        }
    }
}
