using Domain.Entities;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.DbContext
{
    public class DbInitializer
    {
        private readonly ContractOptions _contractOptions;
        private readonly IDbContextFactory<AutoSalonContext> _factory;

        public DbInitializer(IDbContextFactory<AutoSalonContext> factory, IOptions<ContractOptions> contractOptions)
        {
            _factory = factory;
            _contractOptions = contractOptions.Value;
        }

        public async Task InitializeAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();

            await context.Database.EnsureCreatedAsync();

            if (await context.Cars.AnyAsync())
            {
                await UpdateImagesIfMissing(context);
                return;
            }

            if (!await context.Engines.AnyAsync())
            {
                var enginesSeed = new List<EngineSpec>
                {
                    new() { Name = "2.0 л", Volume = 2.0, Power = 150, FuelType = "Бензин" },
                    new() { Name = "2.5 л", Volume = 2.5, Power = 181, FuelType = "Бензин" },
                    new() { Name = "2.5 Hybrid", Volume = 2.5, Power = 218, FuelType = "Гибрид" }
                };
                await context.Engines.AddRangeAsync(enginesSeed);
            }

            if (!await context.Transmissions.AnyAsync())
            {
                var transmissionsSeed = new List<TransmissionSpec>
                {
                    new() { Name = "CVT", Type = "Вариатор", Gears = 0 },
                    new() { Name = "8AT", Type = "Автомат", Gears = 8 },
                    new() { Name = "6AT", Type = "Автомат", Gears = 6 }
                };
                await context.Transmissions.AddRangeAsync(transmissionsSeed);
            }

            if (!await context.CarModels.AnyAsync())
            {
                var modelsSeed = new List<CarModel>
                {
                    new() { Name = "Camry", BodyType = "Седан", Description = "Динамичный бизнес-седан Toyota." },
                    new() { Name = "RAV4", BodyType = "Кроссовер", Description = "Универсальный городской внедорожник." },
                    new() { Name = "Corolla", BodyType = "Седан", Description = "Надежный седан для города и семьи." }
                };
                await context.CarModels.AddRangeAsync(modelsSeed);
            }

            if (!await context.TrimLevels.AnyAsync())
            {
                var trimsSeed = new List<TrimLevel>
                {
                    new() { Name = "Comfort", PriceDelta = 0, Description = "Базовое оснащение" },
                    new() { Name = "Prestige", PriceDelta = 150000, Description = "Расширенный пакет опций" },
                    new() { Name = "GR Sport", PriceDelta = 280000, Description = "Спортивные акценты" }
                };
                await context.TrimLevels.AddRangeAsync(trimsSeed);
            }

            if (!await context.ExtraServices.AnyAsync())
            {
                var extrasSeed = new List<ExtraService>
                {
                    new() { Name = "КАСКО 1 год", Description = "Полное страхование", Price = 95000 },
                    new() { Name = "Антикоррозийная обработка", Description = "Защита кузова и арок", Price = 28000 },
                    new() { Name = "Комплект зимних шин", Description = "185/65 R16", Price = 52000 },
                    new() { Name = "Тонировка стекол", Description = "Сертифицированная пленка", Price = 15000 }
                };
                await context.ExtraServices.AddRangeAsync(extrasSeed);
            }

            await context.SaveChangesAsync();

            var engines = await context.Engines.AsNoTracking().ToListAsync();
            var transmissions = await context.Transmissions.AsNoTracking().ToListAsync();
            var models = await context.CarModels.AsNoTracking().ToListAsync();
            var trims = await context.TrimLevels.AsNoTracking().ToListAsync();
            var extras = await context.ExtraServices.AsNoTracking().ToListAsync();

            var engine20 = engines.First(e => e.Name == "2.0 л");
            var engine25 = engines.First(e => e.Name == "2.5 л");
            var engine25Hybrid = engines.First(e => e.Name == "2.5 Hybrid");
            var cvt = transmissions.First(t => t.Name == "CVT");
            var at8 = transmissions.First(t => t.Name == "8AT");

            var camryModel = models.First(m => m.Name == "Camry");
            var rav4Model = models.First(m => m.Name == "RAV4");
            var corollaModel = models.First(m => m.Name == "Corolla");

            var comfortTrim = trims.First(t => t.Name == "Comfort");
            var prestigeTrim = trims.First(t => t.Name == "Prestige");
            var grSportTrim = trims.First(t => t.Name == "GR Sport");

            var cars = new List<Car>
            {
                new()
                {
                    Vin = "JTNBB3HK603000111",
                    StockNumber = "T-001",
                    ModelId = camryModel.Id,
                    TrimLevelId = prestigeTrim.Id,
                    EngineSpecId = engine25.Id,
                    TransmissionSpecId = at8.Id,
                    Year = 2024,
                    Color = "Белый перламутр",
                    BasePrice = 2850000,
                    Image = LoadImage("camry.png", "Camry Prestige", Color.FromArgb(29, 53, 87))
                },
                new()
                {
                    Vin = "JTNBB3HK603000112",
                    StockNumber = "T-002",
                    ModelId = camryModel.Id,
                    TrimLevelId = grSportTrim.Id,
                    EngineSpecId = engine25.Id,
                    TransmissionSpecId = at8.Id,
                    Year = 2024,
                    Color = "Черный металлик",
                    BasePrice = 3120000,
                    Image = LoadImage("camry-sport.png", "Camry GR Sport", Color.FromArgb(30, 30, 30))
                },
                new()
                {
                    Vin = "JTMR43FV50D008001",
                    StockNumber = "T-003",
                    ModelId = rav4Model.Id,
                    TrimLevelId = comfortTrim.Id,
                    EngineSpecId = engine20.Id,
                    TransmissionSpecId = cvt.Id,
                    Year = 2024,
                    Color = "Серый графит",
                    BasePrice = 2450000,
                    Image = LoadImage("rav4.png", "RAV4 Comfort", Color.FromArgb(64, 70, 84))
                },
                new()
                {
                    Vin = "JTMR43FV50D008002",
                    StockNumber = "T-004",
                    ModelId = rav4Model.Id,
                    TrimLevelId = prestigeTrim.Id,
                    EngineSpecId = engine25Hybrid.Id,
                    TransmissionSpecId = cvt.Id,
                    Year = 2024,
                    Color = "Синий шторм",
                    BasePrice = 2890000,
                    Image = LoadImage("rav4-hybrid.png", "RAV4 Hybrid", Color.FromArgb(23, 67, 98))
                },
                new()
                {
                    Vin = "JTMZ43FV60L005501",
                    StockNumber = "T-005",
                    ModelId = corollaModel.Id,
                    TrimLevelId = comfortTrim.Id,
                    EngineSpecId = engine20.Id,
                    TransmissionSpecId = cvt.Id,
                    Year = 2023,
                    Color = "Красный бурганди",
                    BasePrice = 1950000,
                    Image = LoadImage("corolla.png", "Corolla", Color.FromArgb(138, 22, 38))
                }
            };

            await context.Cars.AddRangeAsync(cars);
            await context.SaveChangesAsync();

            var customers = new List<Customer>
            {
                new() { FullName = "Татьяна Александровна Второва", Phone = "+7 999 111-22-33", Email = "tatienne_v@.com" },
                new() { FullName = "Второв Алексей Владимирович", Phone = "+7 999 777-88-22", Email = "wert.@example.com" }
            };
            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();

            var prestigeDelta = trims.Single(t => t.Name == "Prestige").PriceDelta;
            var camryWithTrim = cars[0].BasePrice + prestigeDelta;

            var sampleSale = new Sale
            {
                CarId = cars[0].Id,
                CustomerId = customers[0].Id,
                SaleDate = DateTime.UtcNow.AddDays(-5),
                DiscountPercent = 5,
                BasePrice = camryWithTrim,
                ExtrasPrice = 28000,
                TotalPrice = camryWithTrim * 0.95m + 28000,
                ContractPath = Path.Combine(_contractOptions.Directory, "SampleContract.pdf")
            };
            sampleSale.SaleExtraServices.Add(new SaleExtraService
            {
                ExtraServiceId = extras.First().Id,
                Price = extras.First().Price
            });
            cars[0].InStock = false;

            var baseDate = DateTime.UtcNow.Date;
            var testDrives = new[]
            {
                new TestDrive
                {
                    CarId = cars[2].Id,
                    CustomerId = customers[1].Id,
                    Slot = baseDate.AddDays(1).AddHours(14),
                    Notes = "Тест вариатора на трассе"
                },
                new TestDrive
                {
                    CarId = cars[3].Id,
                    CustomerId = customers[0].Id,
                    Slot = baseDate.AddDays(2).AddHours(12),
                    Notes = "Проверка гибридной установки"
                }
            };

            await context.Sales.AddAsync(sampleSale);
            await context.TestDrives.AddRangeAsync(testDrives);
            await context.SaveChangesAsync();
        }

        private byte[] LoadImage(string fileName, string label, Color color)
        {
            var assetsDir = Path.Combine(AppContext.BaseDirectory, "Assets");
            var fullPath = Path.Combine(assetsDir, fileName);
            if (File.Exists(fullPath))
            {
                return File.ReadAllBytes(fullPath);
            }

            // Пробуем любые поддерживаемые расширения для указанного базового имени.
            var baseName = Path.GetFileNameWithoutExtension(fileName);
            var tryExtensions = new[] { ".png", ".jpg", ".jpeg" };
            foreach (var ext in tryExtensions)
            {
                var candidate = Path.Combine(assetsDir, baseName + ext);
                if (File.Exists(candidate))
                {
                    return File.ReadAllBytes(candidate);
                }
            }

            return GeneratePlaceholder(label, color);
        }

        private static byte[] GeneratePlaceholder(string label, Color color)
        {
            using var bitmap = new Bitmap(720, 420);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(color);

            using var glass = new SolidBrush(Color.FromArgb(80, Color.White));
            graphics.FillRectangle(glass, 40, 40, bitmap.Width - 80, bitmap.Height - 80);

            using var font = new Font("Segoe UI", 28, FontStyle.Bold, GraphicsUnit.Pixel);
            using var textBrush = new SolidBrush(Color.White);
            var size = graphics.MeasureString(label, font);
            graphics.DrawString(label, font, textBrush, (bitmap.Width - size.Width) / 2, (bitmap.Height - size.Height) / 2);

            using var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        private async Task UpdateImagesIfMissing(AutoSalonContext context)
        {
            var cars = await context.Cars
                .Include(c => c.Model)
                .Include(c => c.TrimLevel)
                .Include(c => c.Engine)
                .ToListAsync();

            foreach (var car in cars)
            {
                var fileName = GetImageFileName(car);
                car.Image = LoadImage(fileName, $"{car.Model.Name} {car.TrimLevel.Name}", GetColorForModel(car.Model.Name));
            }

            await context.SaveChangesAsync();
        }

        private static string GetImageFileName(Car car)
        {
            var model = car.Model.Name;
            if (model.Equals("Camry", StringComparison.OrdinalIgnoreCase))
            {
                return car.TrimLevel.Name.Contains("GR", StringComparison.OrdinalIgnoreCase)
                    ? "camry-sport.png"
                    : "camry.png";
            }

            if (model.Equals("RAV4", StringComparison.OrdinalIgnoreCase))
            {
                return car.Engine.FuelType.Contains("гибрид", StringComparison.OrdinalIgnoreCase) ||
                       car.Engine.Name.Contains("Hybrid", StringComparison.OrdinalIgnoreCase)
                    ? "rav4-hybrid.png"
                    : "rav4.png";
            }

            if (model.Equals("Corolla", StringComparison.OrdinalIgnoreCase))
            {
                return "corolla.png";
            }

            return "camry.png";
        }

        private static Color GetColorForModel(string model)
        {
            return model.ToLowerInvariant() switch
            {
                "camry" => Color.FromArgb(29, 53, 87),
                "rav4" => Color.FromArgb(64, 70, 84),
                "corolla" => Color.FromArgb(138, 22, 38),
                _ => Color.FromArgb(40, 60, 80)
            };
        }
    }
}
