using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Dto;
using Domain.Entities;

namespace Auto.Services.Interfaces
{
    public interface ISalesService
    {
        PriceBreakdown BuildPrice(Car car, IEnumerable<ExtraService> extras, double discountPercent);
        Task<SaleSnapshotDto> CreateSaleAsync(Car car, Customer customer, IEnumerable<ExtraService> extras, double discountPercent);
        Task<IReadOnlyList<SaleSnapshotDto>> GetSalesAsync(int take = 30);
    }
}
