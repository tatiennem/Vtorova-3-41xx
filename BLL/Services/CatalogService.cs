using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.Dto;
using DomainModel;
using DAL.DbContext;
using Microsoft.EntityFrameworkCore;
using Interfaces.Services;
using DAL.Interfaces;

namespace Infrastructure.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IDbRepos _repos;

        public CatalogService(IDbRepos repos)
        {
            _repos = repos;
        }

        public async Task<IReadOnlyList<CarInfoDto>> GetCarsAsync(string? modelFilter = null, bool onlyAvailable = false)
        {

            var query = _repos.Cars
                .Include(c => c.Model)
                .Include(c => c.TrimLevel)
                .Include(c => c.Engine)
                .Include(c => c.Transmission)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(modelFilter))
            {
                query = query.Where(c => c.Model.Name == modelFilter);
            }

            if (onlyAvailable)
            {
                query = query.Where(c => c.InStock);
            }

            var cars = await query
                .OrderBy(c => c.Model.Name)
                .ThenByDescending(c => c.Year)
                .ThenBy(c => c.TrimLevel.Name)
                .ToListAsync();

            return cars.Select(c => new CarInfoDto
            {
                Id = c.Id,
                Model = c.Model.Name,
                Trim = c.TrimLevel.Name,
                Color = c.Color,
                Year = c.Year,
                BasePrice = c.BasePrice + c.TrimLevel.PriceDelta,
                Engine = $"{c.Engine.Name} ({c.Engine.Power} л.с., {c.Engine.FuelType})",
                Transmission = $"{c.Transmission.Name} {c.Transmission.Type}",
                Image = c.Image,
                InStock = c.InStock
            }).ToList();
        }

        public async Task<Car?> GetCarAsync(int id)
        {

            return await _repos.Cars
                .Include(c => c.Model)
                .Include(c => c.TrimLevel)
                .Include(c => c.Engine)
                .Include(c => c.Transmission)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IReadOnlyList<CarModel>> GetModelsAsync()
        {
            return await _repos.CarModels.AsNoTracking().OrderBy(m => m.Name).ToListAsync();
        }

        public async Task<IReadOnlyList<TrimLevel>> GetTrimsAsync()
        {
            return await _repos.TrimLevels.AsNoTracking().OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<IReadOnlyList<ExtraService>> GetExtraServicesAsync()
        {
            return await _repos.ExtraServices.AsNoTracking().OrderBy(e => e.Price).ToListAsync();
        }

        public async Task<IReadOnlyList<Customer>> GetCustomersAsync()
        {

            return await _repos.Customers
                .AsNoTracking()
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }
    }
}
