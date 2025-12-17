using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModel;

namespace DAL.Interfaces
{
    public interface IDbRepos
    {
        IQueryable<Car> Cars { get; }
        IQueryable<CarModel> CarModels { get; }
        IQueryable<TrimLevel> TrimLevels { get; }
        IQueryable<EngineSpec> Engines { get; }
        IQueryable<TransmissionSpec> Transmissions { get; }
        IQueryable<Customer> Customers { get; }
        IQueryable<ExtraService> ExtraServices { get; }
        IQueryable<Sale> Sales { get; }
        IQueryable<SaleExtraService> SaleExtraServices { get; }
        IQueryable<TestDrive> TestDrives { get; }

        // Методы для работы с машинами
        Task<Car?> GetCarAsync(int id);
        Task<List<Car>> GetCarsAsync();
        Task CreateCarAsync(Car car);
        Task UpdateCarAsync(Car car);
        Task DeleteCarAsync(int id);

        // Сохранение изменений
        Task<int> SaveAsync();
        int Save();
    }
}