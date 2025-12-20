using System.Threading.Tasks;
using Domain.Entities;

namespace Auto.Services.Interfaces
{
    public interface IContractService
    {
        Task<string> CreateContractAsync(Sale sale);
    }
}
