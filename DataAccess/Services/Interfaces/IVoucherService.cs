using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IVoucherService
    {
        Task<IEnumerable<VoucherResponse>> GetAllVouchersAsync();
        Task<CreateVoucherResponse> CreateVoucherAsync(CreateVoucherRequest request);
        Task<VoucherResponse> GetVoucherByIdAsync(int id);
        Task<VoucherResponse> UpdateVoucherAsync(int id, UpdateVoucherRequest request);
        Task<bool> DeleteVoucherAsync(int id);
        Task<bool> IsVoucherCodeUniqueAsync(string voucherCode);
        Task<int> CreateOrUpdateVoucher(CreateOrUpdateVoucherRequest request);
        Task<List<CreateOrUpdateVoucherRequest>> GetAllVouchers();
        Task<VoucherResponse> ApplyVoucher(VoucherRequest voucherRequest);
    }
}
