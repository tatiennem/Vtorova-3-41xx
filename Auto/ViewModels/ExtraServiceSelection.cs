using Domain.Entities;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Auto.ViewModels
{
    public partial class ExtraServiceSelection : ObservableObject
    {
        public ExtraServiceSelection(ExtraService service)
        {
            Service = service;
        }

        public ExtraService Service { get; }

        [ObservableProperty] private bool _isSelected;
    }
}
