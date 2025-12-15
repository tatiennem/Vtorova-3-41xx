using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Dto;
using DomainModel;

namespace Interfaces.Services
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
