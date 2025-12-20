using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Auto.Services.Interfaces;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Auto.Services
{
    public class ContractService : IContractService
    {
        private readonly ContractOptions _options;

        public ContractService(IOptions<ContractOptions> options)
        {
            _options = options.Value;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public Task<string> CreateContractAsync(Sale sale)
        {
            var directory = Path.Combine(AppContext.BaseDirectory, _options.Directory);
            Directory.CreateDirectory(directory);

            var fileName = $"Contract_{sale.Id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var path = Path.Combine(directory, fileName);

            var culture = new CultureInfo("ru-RU");

            var extras = sale.SaleExtraServices.Select(x => new
            {
                x.ExtraService.Name,
                x.Price
            }).ToList();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Header()
                        .Text($"Договор купли-продажи № {sale.Id}")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .Column(col =>
                        {
                            col.Item().Text($"Дата: {sale.SaleDate.ToLocalTime():d} | Менеджер: Автосалон Toyota");
                            col.Item().Text($"Покупатель: {sale.Customer.FullName} ({sale.Customer.Phone}, {sale.Customer.Email})");
                            col.Item().PaddingTop(10).Text("Автомобиль").SemiBold();
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Cell().Element(CellHeader).Text("Модель");
                                table.Cell().Text($"{sale.Car.Model.Name} {sale.Car.TrimLevel.Name}");

                                table.Cell().Element(CellHeader).Text("Цвет");
                                table.Cell().Text($"{sale.Car.Color}, {sale.Car.Year} г.");

                                table.Cell().Element(CellHeader).Text("Двигатель");
                                table.Cell().Text($"{sale.Car.Engine.Name}, {sale.Car.Engine.Power} л.с., {sale.Car.Engine.FuelType}");

                                table.Cell().Element(CellHeader).Text("Трансмиссия");
                                table.Cell().Text($"{sale.Car.Transmission.Name} {sale.Car.Transmission.Type}");

                                table.Cell().Element(CellHeader).Text("VIN");
                                table.Cell().Text(sale.Car.Vin);
                            });

                            if (extras.Any())
                            {
                                col.Item().PaddingTop(10).Text("Дополнительные услуги").SemiBold();
                                col.Item().Table(t =>
                                {
                                    t.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(3);
                                        c.RelativeColumn();
                                    });
                                    t.Cell().Element(CellHeader).Text("Услуга");
                                    t.Cell().Element(CellHeader).Text("Стоимость");
                                    foreach (var extra in extras)
                                    {
                                        t.Cell().Text(extra.Name);
                                        t.Cell().Text(extra.Price.ToString("C0", culture));
                                    }
                                });
                            }

                            col.Item().PaddingTop(10).Text("Финальная стоимость").SemiBold();
                            col.Item().Table(t =>
                            {
                                t.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn();
                                    c.RelativeColumn();
                                });
                                t.Cell().Element(CellHeader).Text("Базовая цена");
                                t.Cell().Text(sale.BasePrice.ToString("C0", culture));
                                t.Cell().Element(CellHeader).Text("Скидка");
                                t.Cell().Text($"{sale.DiscountPercent}%");
                                t.Cell().Element(CellHeader).Text("Доп. услуги");
                                t.Cell().Text(sale.ExtrasPrice.ToString("C0", culture));
                                t.Cell().Element(CellHeader).Text("Итого к оплате");
                                t.Cell().Text(sale.TotalPrice.ToString("C0", culture));
                            });

                            col.Item().PaddingTop(20).Text("Подпись продавца: _______________________");
                            col.Item().Text("Подпись покупателя: _____________________");
                        });
                });
            }).GeneratePdf(path);

            return Task.FromResult(path);
        }

        private static IContainer CellHeader(IContainer container)
        {
            return container
                .DefaultTextStyle(x => x.SemiBold())
                .PaddingVertical(4)
                .Background(Colors.Grey.Lighten3);
        }
    }
}
