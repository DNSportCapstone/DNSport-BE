using DataAccess.DTOs;
using DataAccess.DTOs.Response;
namespace DataAccess.Services.Interfaces
{
    public interface IRevenueTransactionService
    {
        Task<List<OwnerAmountResponse>> GetOwnerAmountByFieldIdAsync(OwnerAmountRequest request);
    }
}