using Microsoft.EntityFrameworkCore;
using DomainModel;
using DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;

namespace DAL
{
    public class DbReposSQL : IDbRepos
    {
        private readonly AutoSalonContext _context;

        public DbReposSQL(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AutoSalonContext>();
            optionsBuilder.UseNpgsql(connectionString);
            _context = new AutoSalonContext(optionsBuilder.Options);
        }

        // Реализация свойств IQueryable
        public IQueryable<Car> Cars => _context.Cars
            .Include(c => c.Model)
            .Include(c => c.TrimLevel)
            .Include(c => c.Engine)
            .Include(c => c.Transmission)
            .AsQueryable();

        public IQueryable<CarModel> CarModels => _context.CarModels.AsQueryable();
        public IQueryable<TrimLevel> TrimLevels => _context.TrimLevels.AsQueryable();
        public IQueryable<EngineSpec> Engines => _context.Engines.AsQueryable();
        public IQueryable<TransmissionSpec> Transmissions => _context.Transmissions.AsQueryable();
        public IQueryable<Customer> Customers => _context.Customers.AsQueryable();
        public IQueryable<ExtraService> ExtraServices => _context.ExtraServices.AsQueryable();
        public IQueryable<Sale> Sales => _context.Sales.AsQueryable();
        public IQueryable<SaleExtraService> SaleExtraServices => _context.SaleExtraServices.AsQueryable();
        public IQueryable<TestDrive> TestDrives => _context.TestDrives.AsQueryable();

        // Методы для Car
        public async Task<Car?> GetCarAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.Model)
                .Include(c => c.TrimLevel)
                .Include(c => c.Engine)
                .Include(c => c.Transmission)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Car>> GetCarsAsync()
        {
            return await _context.Cars
                .Include(c => c.Model)
                .Include(c => c.TrimLevel)
                .Include(c => c.Engine)
                .Include(c => c.Transmission)
                .ToListAsync();
        }

        public async Task CreateCarAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCarAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }

        // Методы сохранения
        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
        public int Save() => _context.SaveChanges();
    }
}