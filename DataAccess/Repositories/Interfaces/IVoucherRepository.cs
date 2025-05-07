using DataAccess.Model;
using System.Threading.Tasks;
using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;

namespace DataAccess.Repositories.Interfaces
{
    public interface IVoucherRepository
    {
        Task<IEnumerable<VoucherModel>> GetAllVouchersAsync();
        Task<VoucherModel> GetVoucherByIdAsync(int id);
        Task<VoucherModel> CreateVoucherAsync(VoucherModel voucher);
        Task<bool> UpdateVoucherAsync(VoucherModel voucher);
        Task<bool> DeleteVoucherAsync(int id);
        Task<bool> IsVoucherCodeUniqueAsync(string voucherCode);
        Task<int> CreateOrUpdateVoucher(CreateOrUpdateVoucherRequest request);
        Task<List<CreateOrUpdateVoucherRequest>> GetAllVouchers();
        Task<VoucherResponse> ApplyVoucher(VoucherRequest voucherRequest);
    }
}
