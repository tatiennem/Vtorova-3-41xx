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

        // Существующий метод (оставляем как есть)
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

        // Существующий метод (оставляем как есть)
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

        // НОВЫЙ МЕТОД: Выручка по моделям за текущий месяц
        public async Task<IReadOnlyList<ModelMonthRevenue>> GetModelRevenueForCurrentMonthAsync()
        {
            var currentMonthStart = new DateTime(_clock.Today.Year, _clock.Today.Month, 1);
            var nextMonthStart = currentMonthStart.AddMonths(1);

            await using var db = await _factory.CreateDbContextAsync();

            // Получаем общую выручку за месяц
            var totalMonthRevenue = await db.Sales
                .Where(s => s.SaleDate >= currentMonthStart && s.SaleDate < nextMonthStart)
                .SumAsync(s => s.TotalPrice);

            // Получаем данные по моделям
            var modelsData = await db.Sales
                .Where(s => s.SaleDate >= currentMonthStart && s.SaleDate < nextMonthStart)
                .Include(s => s.Car).ThenInclude(c => c.Model)
                .GroupBy(s => s.Car.Model.Name)
                .Select(g => new
                {
                    Model = g.Key,
                    Sold = g.Count(),
                    Revenue = g.Sum(s => s.TotalPrice),
                    AveragePrice = g.Average(s => s.TotalPrice)
                })
                .ToListAsync();

            // Рассчитываем проценты
            var result = modelsData.Select(m => new ModelMonthRevenue
            {
                Model = m.Model,
                Sold = m.Sold,
                Revenue = m.Revenue,
                AveragePrice = m.AveragePrice,
                Percentage = totalMonthRevenue > 0 ? (m.Revenue / totalMonthRevenue) * 100 : 0
            })
            .OrderByDescending(m => m.Revenue)
            .ToList();

            return result;
        }
        // Удаляем старый метод (если был)
        public Task<IReadOnlyList<TestDriveScheduleItem>> GetUpcomingTestDrivesAsync(int days)
        {
            // Метод можно удалить или оставить пустым
            return Task.FromResult<IReadOnlyList<TestDriveScheduleItem>>(new List<TestDriveScheduleItem>());
        }
    }
}