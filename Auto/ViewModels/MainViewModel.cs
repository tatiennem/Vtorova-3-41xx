using Auto.ViewModels;
using Auto.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Auto.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty] private object? _currentViewModel;

        public ObservableCollection<NavigationItem> NavigationItems { get; }

        public IRelayCommand<NavigationItem?> NavigateCommand { get; }

        public MainViewModel(CatalogViewModel catalog, TestDriveViewModel testDrive, ReportsViewModel reports)
        {
            NavigationItems = new ObservableCollection<NavigationItem>
            {
                new() { Title = "Каталог", Description = "Авто и комплектации", Icon = "\uE7C3", ViewModel = catalog },
                new() { Title = "Тест-драйв", Description = "Запись клиентов", Icon = "\uE8EB", ViewModel = testDrive },
                new() { Title = "Отчеты", Description = "Продажи и загрузка", Icon = "\uE9D9", ViewModel = reports }
            };

            _currentViewModel = catalog;
            NavigateCommand = new RelayCommand<NavigationItem?>(Navigate);
        }

        private void Navigate(NavigationItem? item)
        {
            if (item?.ViewModel == null || ReferenceEquals(CurrentViewModel, item.ViewModel))
            {
                return;
            }

            CurrentViewModel = item.ViewModel;

            if (CurrentViewModel is IRefreshable refreshable)
            {
                _ = refreshable.RefreshAsync();
            }
        }
    }
}
