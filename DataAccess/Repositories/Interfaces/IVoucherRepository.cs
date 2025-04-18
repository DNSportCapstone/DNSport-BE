using DataAccess.DTOs.Request;

namespace DataAccess.Repositories.Interfaces
{
    public interface IVoucherRepository
    {
        Task<int> CreateOrUpdateVoucher(CreateVoucherRequest request);
        Task<List<CreateVoucherRequest>> GetAllVouchers();
    }
}
