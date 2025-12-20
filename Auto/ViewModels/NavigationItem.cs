using CommunityToolkit.Mvvm.ComponentModel;

namespace Auto.ViewModels
{
    public partial class NavigationItem : ObservableObject
    {
        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private string _icon = string.Empty;
        [ObservableProperty] private object? _viewModel;
        [ObservableProperty] private bool _isEnabled = true;
    }
}
