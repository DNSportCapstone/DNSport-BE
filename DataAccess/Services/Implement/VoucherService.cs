using DataAccess.DTOs.Request;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;

namespace DataAccess.Services.Implement
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;
        public VoucherService(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }
        public async Task<int> CreateOrUpdateVoucher(CreateVoucherRequest request)
        {
            return await _voucherRepository.CreateOrUpdateVoucher(request);
        }

        public async Task<List<CreateVoucherRequest>> GetAllVouchers()
        {
            return await _voucherRepository.GetAllVouchers();
        }
    }
}
