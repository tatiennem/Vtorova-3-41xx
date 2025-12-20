using System.Collections.ObjectModel;
using Auto.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Auto.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? _currentViewModel;

        [ObservableProperty]
        private NavigationItem? _selectedNavigationItem;

        public ObservableCollection<NavigationItem> NavigationItems { get; }

        public MainViewModel(CatalogViewModel catalog)
        {
            // Пока только каталог
            NavigationItems = new ObservableCollection<NavigationItem>
            {
                new() {
                    Title = "Каталог",
                    Description = "Авто и комплектации",
                    Icon = "\uE7C3",
                    ViewModel = catalog,
                    IsEnabled = true
                }
            };

            _currentViewModel = catalog;
            _selectedNavigationItem = NavigationItems[0];
        }
    }
}