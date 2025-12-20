using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DAL.DbContext;
using Domain.Dto;
using Domain.Entities;
using Auto.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Auto.ViewModels
{
    public partial class TestDriveViewModel : ObservableObject, IRefreshable
    {
        private readonly ICatalogService _catalogService;
        private readonly ITestDriveService _testDriveService;
        private readonly INotificationService _notifications;

        [ObservableProperty] private CarInfoDto? _selectedCar;
        [ObservableProperty] private Car? _selectedCarEntity;
        [ObservableProperty] private DateTime _selectedDate = DateTime.Today.AddDays(1);
        [ObservableProperty] private TimeSpan _selectedTime = new TimeSpan(12, 0, 0);
        [ObservableProperty] private string _customerName = string.Empty;
        [ObservableProperty] private string _customerPhone = string.Empty;
        [ObservableProperty] private string _customerEmail = string.Empty;
        [ObservableProperty] private string _notes = "Хочу протестировать авто в городе";
        [ObservableProperty] private Customer? _selectedCustomer;

        public ObservableCollection<CarInfoDto> Cars { get; } = new();
        public ObservableCollection<TestDriveScheduleItem> Upcoming { get; } = new();
        public ObservableCollection<Customer> Customers { get; } = new();

        public IAsyncRelayCommand BookCommand { get; }
        public IRelayCommand NewCustomerCommand { get; }

        private bool _isRefreshing;
        private bool _isBooking;

        public TestDriveViewModel(ICatalogService catalogService, ITestDriveService testDriveService, INotificationService notifications)
        {
            _catalogService = catalogService;
            _testDriveService = testDriveService;
            _notifications = notifications;

            BookCommand = new AsyncRelayCommand(BookAsync);
            NewCustomerCommand = new RelayCommand(ClearCustomer);
        }

        partial void OnSelectedCarChanged(CarInfoDto? value)
        {
            if (value != null)
            {
                _ = LoadCarAsync(value.Id);
            }
        }

        private async Task LoadCarAsync(int id)
        {
            SelectedCarEntity = await _catalogService.GetCarAsync(id);
        }

        public async Task RefreshAsync(bool force = false)
        {
            if (_isRefreshing && !force)
            {
                return;
            }

            _isRefreshing = true;
            var selectedCarId = SelectedCar?.Id;
            var selectedCustomerId = SelectedCustomer?.Id;

            try
            {
                var cars = await _catalogService.GetCarsAsync();
                Cars.Clear();
                foreach (var car in cars)
                {
                    Cars.Add(car);
                }
                SelectedCar = Cars.FirstOrDefault(c => c.Id == selectedCarId) ?? Cars.FirstOrDefault();

                var clients = await _catalogService.GetCustomersAsync();
                Customers.Clear();
                foreach (var client in clients)
                {
                    Customers.Add(client);
                }
                SelectedCustomer = Customers.FirstOrDefault(c => c.Id == selectedCustomerId) ?? Customers.FirstOrDefault();

                var schedule = await _testDriveService.GetUpcomingAsync(14);
                Upcoming.Clear();
                foreach (var item in schedule)
                {
                    Upcoming.Add(item);
                }
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        private async Task BookAsync()
        {
            if (_isBooking)
            {
                return;
            }
            _isBooking = true;

            if (SelectedCarEntity == null)
            {
                await _notifications.ShowErrorAsync("Выберите автомобиль для тест-драйва.");
                _isBooking = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(CustomerName) || string.IsNullOrWhiteSpace(CustomerPhone))
            {
                await _notifications.ShowErrorAsync("Имя и телефон обязательны для записи.");
                _isBooking = false;
                return;
            }

            var slotLocal = SelectedDate.Date + SelectedTime;
            var slotUtc = DateTime.SpecifyKind(slotLocal, DateTimeKind.Local).ToUniversalTime();

            var customer = new Customer
            {
                FullName = CustomerName.Trim(),
                Phone = CustomerPhone.Trim(),
                Email = CustomerEmail.Trim()
            };

            try
            {
                await _testDriveService.CreateAsync(SelectedCarEntity, customer, slotUtc, Notes.Trim());
                await _notifications.ShowInfoAsync("Клиент записан на тест-драйв.");
                await RefreshAsync();
            }
            catch (Exception ex)
            {
                await _notifications.ShowErrorAsync(ex.Message);
            }
            finally
            {
                _isBooking = false;
            }
        }

        partial void OnSelectedCustomerChanged(Customer? value)
        {
            if (value != null)
            {
                CustomerName = value.FullName;
                CustomerPhone = value.Phone;
                CustomerEmail = value.Email;
            }
        }

        private void ClearCustomer()
        {
            SelectedCustomer = null;
            CustomerName = string.Empty;
            CustomerPhone = string.Empty;
            CustomerEmail = string.Empty;
        }
    }
}
