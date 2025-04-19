using DataAccess.DTOs;
using DataAccess.DTOs.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    public interface IRevenueTransactionRepository
    {
        Task<List<OwnerAmountResponse>> GetOwnerAmountByFieldIdAsync(OwnerAmountRequest request);
    }
}