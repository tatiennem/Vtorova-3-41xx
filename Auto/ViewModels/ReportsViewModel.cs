using System.Collections.ObjectModel;
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

        public ObservableCollection<MonthlySalesReport> MonthlySales { get; } = new();
        public ObservableCollection<ModelSalesReport> ModelSales { get; } = new();
        public ObservableCollection<TestDriveScheduleItem> UpcomingTestDrives { get; } = new();

        public ReportsViewModel(IReportService reportService)
        {
            _reportService = reportService;
        }

        partial void OnMonthsChanged(int value)
        {
            _ = RefreshAsync();
        }

        public async Task RefreshAsync(bool force = false)
        {
            var monthly = await _reportService.GetMonthlySalesAsync(Months);
            MonthlySales.Clear();
            foreach (var item in monthly)
            {
                MonthlySales.Add(item);
            }

            var models = await _reportService.GetModelSalesAsync();
            ModelSales.Clear();
            foreach (var item in models)
            {
                ModelSales.Add(item);
            }

            var drives = await _reportService.GetUpcomingTestDrivesAsync(30);
            UpcomingTestDrives.Clear();
            foreach (var drive in drives)
            {
                UpcomingTestDrives.Add(drive);
            }
        }
    }
}
