using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Dto;
using Domain.Entities;

namespace Auto.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<IReadOnlyList<CarInfoDto>> GetCarsAsync(string? modelFilter = null, bool onlyAvailable = false);
        Task<Car?> GetCarAsync(int id);
        Task<IReadOnlyList<CarModel>> GetModelsAsync();
        Task<IReadOnlyList<TrimLevel>> GetTrimsAsync();
        Task<IReadOnlyList<ExtraService>> GetExtraServicesAsync();
        Task<IReadOnlyList<Customer>> GetCustomersAsync();
    }
}
