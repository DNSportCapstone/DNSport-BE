using DataAccess.DTOs.Response;
using DataAccess.DTOs;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;


namespace DataAccess.Services.Implement
{
    public class RevenueTransactionService : IRevenueTransactionService
    {
        private readonly IRevenueTransactionRepository _revenueTransactionRepository;

        public RevenueTransactionService(IRevenueTransactionRepository revenueTransactionRepository)
        {
            _revenueTransactionRepository = revenueTransactionRepository;
        }

        public async Task<List<OwnerAmountResponse>> GetOwnerAmountByFieldIdAsync(OwnerAmountRequest request)
        {
            if (request == null || request.FieldId <= 0)
            {
                throw new ArgumentException("Invalid FieldId.");
            }

            var result = await _revenueTransactionRepository.GetOwnerAmountByFieldIdAsync(request);
            return result;
        }
    }
}