using DataAccess.DTOs.Request;

namespace DataAccess.Services.Interfaces
{
    public interface IVoucherService
    {
        Task<int> CreateOrUpdateVoucher(CreateVoucherRequest request);
        Task<List<CreateVoucherRequest>> GetAllVouchers();
    }
}
