using Interfaces.Dto;
using Interfaces.Services;
using DomainModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Auto.ViewModels
{
    public partial class CatalogViewModel : ObservableObject, IRefreshable
    {
        private readonly ICatalogService _catalogService;

        [ObservableProperty] private CarInfoDto? _selectedCar;
        [ObservableProperty] private Car? _selectedCarEntity;
        [ObservableProperty] private string _modelFilter = "Все модели";
        [ObservableProperty] private bool _onlyAvailable = true;

        private bool _isRefreshing;

        public ObservableCollection<CarInfoDto> Cars { get; } = new();
        public ObservableCollection<string> ModelFilters { get; } = new();

        public CatalogViewModel(ICatalogService catalogService)
        {
            _catalogService = catalogService;

            // ЗАГРУЖАЕМ ДАННЫЕ ПРИ СОЗДАНИИ ViewModel
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await RefreshAsync();
        }

        partial void OnSelectedCarChanged(CarInfoDto? value)
        {
            if (value != null)
            {
                _ = LoadCarDetailsAsync(value.Id);
            }
        }

        partial void OnOnlyAvailableChanged(bool value)
        {
            _ = RefreshAsync();
        }

        partial void OnModelFilterChanged(string value)
        {
            _ = RefreshAsync();
        }

        private async Task LoadCarDetailsAsync(int carId)
        {
            SelectedCarEntity = await _catalogService.GetCarAsync(carId);
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
            }
            finally
            {
                _isRefreshing = false;
            }
        }
    }
}