using System;
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
    public class SalesService : ISalesService
    {
        private readonly IDbContextFactory<AutoSalonContext> _factory;
        private readonly IContractService _contractService;
        private readonly IDateTimeProvider _clock;

        private const double MaxDiscount = 20.0;

        public SalesService(IDbContextFactory<AutoSalonContext> factory, IContractService contractService, IDateTimeProvider clock)
        {
            _factory = factory;
            _contractService = contractService;
            _clock = clock;
        }

        public PriceBreakdown BuildPrice(Car car, IEnumerable<ExtraService> extras, double discountPercent)
        {
            var normalized = Math.Clamp(discountPercent, 0, MaxDiscount);
            var basePrice = car.BasePrice + car.TrimLevel.PriceDelta;
            var extrasPrice = extras.Sum(e => e.Price);

            return new PriceBreakdown
            {
                BasePrice = basePrice,
                ExtrasPrice = extrasPrice,
                DiscountPercent = normalized
            };
        }

        public async Task<SaleSnapshotDto> CreateSaleAsync(Car car, Customer customer, IEnumerable<ExtraService> extras, double discountPercent)
        {
            var discount = Math.Clamp(discountPercent, 0, MaxDiscount);
            var extraList = extras.ToList();

            // Attach customer or reuse existing by phone/email.
            await using var db = await _factory.CreateDbContextAsync();

            var existingCustomer = await db.Customers
                .FirstOrDefaultAsync(c => c.Phone == customer.Phone || c.Email == customer.Email);

            if (existingCustomer != null)
            {
                existingCustomer.FullName = customer.FullName;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.Email = customer.Email;
                customer = existingCustomer;
            }
            else
            {
                db.Customers.Add(customer);
            }

            // Перечитываем машину с навигациями в текущем контексте
            var carFromDb = await db.Cars
                .Include(c => c.Model)
                .Include(c => c.TrimLevel)
                .Include(c => c.Engine)
                .Include(c => c.Transmission)
                .FirstAsync(c => c.Id == car.Id);

            var breakdown = BuildPrice(carFromDb, extraList, discount);
            var sale = new Sale
            {
                Car = carFromDb,
                Customer = customer,
                SaleDate = _clock.UtcNow,
                DiscountPercent = discount,
                BasePrice = breakdown.BasePrice,
                ExtrasPrice = breakdown.ExtrasPrice,
                TotalPrice = breakdown.Total
            };

            foreach (var extra in extraList)
            {
                sale.SaleExtraServices.Add(new SaleExtraService
                {
                    ExtraServiceId = extra.Id,
                    Price = extra.Price
                });
            }

            carFromDb.InStock = false;
            db.Sales.Add(sale);
            await db.SaveChangesAsync();

            // Подтягиваем все навигации для договора.
            var saleWithExtras = await db.Sales
                .Include(s => s.SaleExtraServices).ThenInclude(se => se.ExtraService)
                .Include(s => s.Customer)
                .Include(s => s.Car).ThenInclude(c => c.Model)
                .Include(s => s.Car).ThenInclude(c => c.TrimLevel)
                .Include(s => s.Car).ThenInclude(c => c.Engine)
                .Include(s => s.Car).ThenInclude(c => c.Transmission)
                .FirstAsync(s => s.Id == sale.Id);

            // Create PDF contract and persist path.
            saleWithExtras.ContractPath = await _contractService.CreateContractAsync(saleWithExtras);
            await db.SaveChangesAsync();

            return new SaleSnapshotDto
            {
                Id = saleWithExtras.Id,
                Car = $"{saleWithExtras.Car.Model.Name} {saleWithExtras.Car.TrimLevel.Name} {saleWithExtras.Car.Color}",
                Customer = saleWithExtras.Customer.FullName,
                Date = saleWithExtras.SaleDate,
                DiscountPercent = saleWithExtras.DiscountPercent,
                TotalPrice = saleWithExtras.TotalPrice,
                ContractPath = saleWithExtras.ContractPath
            };
        }

        public async Task<IReadOnlyList<SaleSnapshotDto>> GetSalesAsync(int take = 100)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var rows = await db.Sales
                .Include(s => s.Car).ThenInclude(c => c.Model)
                .Include(s => s.Car).ThenInclude(c => c.TrimLevel)
                .Include(s => s.Customer)
                .OrderByDescending(s => s.SaleDate)
                .Take(take)
                .ToListAsync();

            return rows
                .Select(s => new SaleSnapshotDto
                {
                    Id = s.Id,
                    Car = $"{s.Car.Model.Name} {s.Car.TrimLevel.Name} {s.Car.Color}",
                    Customer = s.Customer.FullName,
                    Date = s.SaleDate,
                    DiscountPercent = s.DiscountPercent,
                    TotalPrice = s.TotalPrice,
                    ContractPath = s.ContractPath
                })
                .ToList();
        }
    }
}
