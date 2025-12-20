using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Auto.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Domain.Dto;
using Domain.Entities;
using Auto.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Auto.ViewModels
{
    public partial class CatalogViewModel : ObservableObject, IRefreshable
    {
        private readonly ICatalogService _catalogService;
        private readonly ISalesService _salesService;
        private readonly INotificationService _notifications;

        [ObservableProperty] private CarInfoDto? _selectedCar;
        [ObservableProperty] private Car? _selectedCarEntity;
        [ObservableProperty] private double _discountPercent = 5;
        [ObservableProperty] private decimal _extrasTotal;
        [ObservableProperty] private string _customerName = string.Empty;
        [ObservableProperty] private string _customerPhone = string.Empty;
        [ObservableProperty] private string _customerEmail = string.Empty;
        [ObservableProperty] private Customer? _selectedCustomer;
        [ObservableProperty] private string _modelFilter = "Все модели";
        [ObservableProperty] private bool _onlyAvailable = true;

        private PriceBreakdown? _price;
        private bool _isRefreshing;
        private bool _isPurchasing;

        public ObservableCollection<CarInfoDto> Cars { get; } = new();
        public ObservableCollection<ExtraServiceSelection> ExtraServices { get; } = new();
        public ObservableCollection<SaleSnapshotDto> RecentSales { get; } = new();
        public ObservableCollection<string> ModelFilters { get; } = new();
        public ObservableCollection<Customer> Customers { get; } = new();

        public IAsyncRelayCommand PurchaseCommand { get; }
        public IRelayCommand NewCustomerCommand { get; }

        public decimal FinalPrice => _price?.Total ?? 0m;
        public decimal BasePrice => _price?.BasePrice ?? 0m;

        public CatalogViewModel(ICatalogService catalogService, ISalesService salesService, INotificationService notifications)
        {
            _catalogService = catalogService;
            _salesService = salesService;
            _notifications = notifications;

            PurchaseCommand = new AsyncRelayCommand(PurchaseAsync);
            NewCustomerCommand = new RelayCommand(ClearCustomer);


        }

        partial void OnSelectedCarChanged(CarInfoDto? value)
        {
            if (value != null)
            {
                _ = LoadCarDetailsAsync(value.Id);
            }
        }

        partial void OnDiscountPercentChanged(double value)
        {
            UpdatePrice();
        }

        partial void OnOnlyAvailableChanged(bool value)
        {
            _ = RefreshAsync();
        }

        partial void OnModelFilterChanged(string value)
        {
            _ = RefreshAsync();
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

        private async Task LoadCarDetailsAsync(int carId)
        {
            SelectedCarEntity = await _catalogService.GetCarAsync(carId);
            UpdatePrice();
        }

        public async Task RefreshAsync(bool force = false)
        {
            if (_isRefreshing && !force)
            {
                return;
            }

            _isRefreshing = true;
            try
            {
                var selectedCarId = SelectedCar?.Id;
                var selectedCustomerId = SelectedCustomer?.Id;
                var filter = ModelFilter == "Все модели" ? null : ModelFilter;
                var cars = await _catalogService.GetCarsAsync(filter, OnlyAvailable);

                Cars.Clear();
                foreach (var car in cars)
                {
                    Cars.Add(car);
                }
                SelectedCar = Cars.FirstOrDefault(c => c.Id == selectedCarId) ?? Cars.FirstOrDefault();

                var models = await _catalogService.GetModelsAsync();
                ModelFilters.Clear();
                ModelFilters.Add("Все модели");
                foreach (var model in models)
                {
                    ModelFilters.Add(model.Name);
                }

                var customers = await _catalogService.GetCustomersAsync();
                Customers.Clear();
                foreach (var c in customers)
                {
                    Customers.Add(c);
                }
                SelectedCustomer = Customers.FirstOrDefault(c => c.Id == selectedCustomerId) ?? Customers.FirstOrDefault();

                var extras = await _catalogService.GetExtraServicesAsync();
                ExtraServices.Clear();
                foreach (var extra in extras)
                {
                    var vm = new ExtraServiceSelection(extra);
                    vm.PropertyChanged += OnExtraSelectionChanged;
                    ExtraServices.Add(vm);
                }

                var sales = await _salesService.GetSalesAsync(12);
                RecentSales.Clear();
                foreach (var sale in sales)
                {
                    RecentSales.Add(sale);
                }
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        private void OnExtraSelectionChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExtraServiceSelection.IsSelected))
            {
                UpdatePrice();
            }
        }

        private void UpdatePrice()
        {
            if (SelectedCarEntity == null)
            {
                _price = null;
                ExtrasTotal = 0;
                OnPropertyChanged(nameof(FinalPrice));
                OnPropertyChanged(nameof(BasePrice));
                return;
            }

            var extras = ExtraServices.Where(x => x.IsSelected).Select(x => x.Service).ToList();
            _price = _salesService.BuildPrice(SelectedCarEntity, extras, DiscountPercent);
            ExtrasTotal = extras.Sum(x => x.Price);
            OnPropertyChanged(nameof(FinalPrice));
            OnPropertyChanged(nameof(BasePrice));
        }

        private async Task PurchaseAsync()
        {
            if (_isPurchasing)
            {
                return;
            }

            _isPurchasing = true;
            if (SelectedCarEntity == null)
            {
                await _notifications.ShowErrorAsync("Выберите автомобиль для оформления.");
                _isPurchasing = false;
                return;
            }

            if (!SelectedCarEntity.InStock)
            {
                await _notifications.ShowErrorAsync("Автомобиль уже продан или зарезервирован.");
                _isPurchasing = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(CustomerName) || string.IsNullOrWhiteSpace(CustomerPhone))
            {
                await _notifications.ShowErrorAsync("Заполните имя и телефон покупателя.");
                _isPurchasing = false;
                return;
            }

            var extras = ExtraServices.Where(x => x.IsSelected).Select(x => x.Service).ToList();
            var customer = new Customer
            {
                FullName = CustomerName.Trim(),
                Phone = CustomerPhone.Trim(),
                Email = CustomerEmail.Trim()
            };

            try
            {
                var sale = await _salesService.CreateSaleAsync(SelectedCarEntity, customer, extras, DiscountPercent);
                await _notifications.ShowInfoAsync($"Продажа оформлена. Договор сохранён: {sale.ContractPath}");
                await RefreshAsync();
            }
            catch (Exception ex)
            {
                await _notifications.ShowErrorAsync($"Не удалось оформить продажу: {ex.Message}");
            }
            finally
            {
                _isPurchasing = false;
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
