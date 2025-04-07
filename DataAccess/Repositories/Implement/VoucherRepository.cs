using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implement
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly Db12353Context _context;
        public VoucherRepository(Db12353Context context)
        {
            _context = context;
        }
        public async Task<int> CreateOrUpdateVoucher(CreateVoucherRequest model)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.VoucherId == model.VoucherId) ?? new Voucher();
            voucher.VoucherCode = model.VoucherCode;
            voucher.DiscountPercentage = model.DiscountPercentage;
            voucher.ExpiryDate = model.ExpirationDate;

            if (voucher.VoucherId == 0)
            {
                await _context.Vouchers.AddAsync(voucher);
            }
            await _context.SaveChangesAsync();
            return voucher.VoucherId;
        }

        public async Task<List<CreateVoucherRequest>> GetAllVouchers()
        {
            var result = await _context.Vouchers
                .Select(v => new CreateVoucherRequest
                {
                    VoucherId = v.VoucherId,
                    VoucherCode = v.VoucherCode,
                    DiscountPercentage = (int)v.DiscountPercentage,
                    ExpirationDate = (DateTime)v.ExpiryDate
                })
                .ToListAsync();
            return result;
        }
    }
}
