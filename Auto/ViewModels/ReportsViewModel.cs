using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Domain.Dto;
using DAL.DbContext;
using Auto.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Auto.ViewModels
{
    public partial class ReportsViewModel : ObservableObject, IRefreshable
    {
        private readonly IReportService _reportService;

        [ObservableProperty] private int _months = 6;
        [ObservableProperty] private string _currentMonth;
        [ObservableProperty] private decimal _totalMonthRevenue;
        [ObservableProperty] private int _totalMonthSales;
        [ObservableProperty] private string _topModelThisMonth;

        public ObservableCollection<MonthlySalesReport> MonthlySales { get; } = new();
        public ObservableCollection<ModelMonthRevenue> ModelsThisMonth { get; } = new(); // Новая коллекция

        public ReportsViewModel(IReportService reportService)
        {
            _reportService = reportService;
            CurrentMonth = DateTime.Today.ToString("MMMM yyyy");
        }

        partial void OnMonthsChanged(int value)
        {
            _ = RefreshAsync();
        }

        public async Task RefreshAsync(bool force = false)
        {
            // 1. Продажи по месяцам (как было)
            var monthly = await _reportService.GetMonthlySalesAsync(Months);
            MonthlySales.Clear();
            foreach (var item in monthly)
            {
                MonthlySales.Add(item);
            }

            // 2. НОВОЕ: Выручка по моделям за текущий месяц
            var currentMonthData = await _reportService.GetModelRevenueForCurrentMonthAsync();
            ModelsThisMonth.Clear();

            foreach (var item in currentMonthData.OrderByDescending(m => m.Revenue))
            {
                ModelsThisMonth.Add(item);
            }

            // 3. Рассчитываем статистику текущего месяца
            CalculateMonthStatistics(currentMonthData);
        }

        private void CalculateMonthStatistics(IReadOnlyList<ModelMonthRevenue> monthData)
        {
            TotalMonthRevenue = monthData.Sum(m => m.Revenue);
            TotalMonthSales = monthData.Sum(m => m.Sold);

            var topModel = monthData.OrderByDescending(m => m.Revenue).FirstOrDefault();
            TopModelThisMonth = topModel?.Model ?? "Нет данных";
        }
    }
}