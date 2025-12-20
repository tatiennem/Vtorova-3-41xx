using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.DbContext;
using Domain.Dto;
using Domain.Entities;
using Auto.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Auto.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IDbContextFactory<AutoSalonContext> _factory;

        public CatalogService(IDbContextFactory<AutoSalonContext> factory)
        {
            _factory = factory;
        }

        public async Task<IReadOnlyList<CarInfoDto>> GetCarsAsync(string? modelFilter = null, bool onlyAvailable = false)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var query = db.Cars
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
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Cars
                .Include(c => c.Model)
                .Include(c => c.TrimLevel)
                .Include(c => c.Engine)
                .Include(c => c.Transmission)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IReadOnlyList<CarModel>> GetModelsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.CarModels.AsNoTracking().OrderBy(m => m.Name).ToListAsync();
        }

        public async Task<IReadOnlyList<TrimLevel>> GetTrimsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.TrimLevels.AsNoTracking().OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<IReadOnlyList<ExtraService>> GetExtraServicesAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.ExtraServices.AsNoTracking().OrderBy(e => e.Price).ToListAsync();
        }

        public async Task<IReadOnlyList<Customer>> GetCustomersAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Customers
                .AsNoTracking()
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }
    }
}
