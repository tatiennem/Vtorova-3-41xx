using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.DbContext;
using Domain.Dto;
using Domain.Entities;
using Auto.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonToyota.Services
{
    public class TestDriveService : ITestDriveService
    {
        private readonly IDbContextFactory<AutoSalonContext> _factory;
        private readonly IDateTimeProvider _clock;

        public TestDriveService(IDbContextFactory<AutoSalonContext> factory, IDateTimeProvider clock)
        {
            _factory = factory;
            _clock = clock;
        }

        public async Task<TestDrive> CreateAsync(Car car, Customer customer, DateTime slot, string notes)
        {
            var slotUtc = slot.Kind == DateTimeKind.Utc
                ? slot
                : DateTime.SpecifyKind(slot, DateTimeKind.Local).ToUniversalTime();

            if (slotUtc < _clock.UtcNow.AddHours(1))
            {
                throw new InvalidOperationException("Заявка должна быть минимум за час до начала.");
            }

            // Reuse customer if already exists.
            await using var db = await _factory.CreateDbContextAsync();

            var existingCustomer = await db.Customers
                .FirstOrDefaultAsync(c => c.Phone == customer.Phone || c.Email == customer.Email);
            if (existingCustomer != null)
            {
                customer = existingCustomer;
            }
            else
            {
                db.Customers.Add(customer);
            }

            var startWindow = slotUtc.AddMinutes(-60);
            var endWindow = slotUtc.AddMinutes(60);
            var overlap = await db.TestDrives
                .AnyAsync(t => t.CarId == car.Id && t.Slot >= startWindow && t.Slot <= endWindow);

            if (overlap)
            {
                throw new InvalidOperationException("У авто уже есть запись на выбранное время.");
            }

            var entity = new TestDrive
            {
                CarId = car.Id,
                CustomerId = customer.Id,
                Customer = customer,
                Slot = slotUtc,
                Notes = notes
            };

            db.TestDrives.Add(entity);
            await db.SaveChangesAsync();
            return entity;
        }

        public async Task<IReadOnlyList<TestDriveScheduleItem>> GetUpcomingAsync(int days)
        {
            var from = _clock.Today;
            var to = from.AddDays(days);

            await using var db = await _factory.CreateDbContextAsync();

            var items = await db.TestDrives
                .Include(t => t.Car).ThenInclude(c => c.Model)
                .Include(t => t.Customer)
                .Where(t => t.Slot >= from && t.Slot <= to)
                .OrderBy(t => t.Slot)
                .ToListAsync();

            return items.Select(t => new TestDriveScheduleItem
            {
                Slot = DateTime.SpecifyKind(t.Slot, DateTimeKind.Utc).ToLocalTime(),
                Car = $"{t.Car.Model.Name} {t.Car.Color}",
                Customer = t.Customer.FullName,
                Phone = t.Customer.Phone
            }).ToList();
        }
    }
}
