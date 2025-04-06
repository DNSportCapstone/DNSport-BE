using AutoMapper;
using BusinessObject.Models;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class VoucherRepository : IVoucherRepository
{
    private readonly Db12353Context _dbcontext;
    private readonly IMapper _mapper;

    public VoucherRepository(Db12353Context context, IMapper mapper)
    {
        _dbcontext = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VoucherModel>> GetAllVouchersAsync()
    {
        var vouchers = await _dbcontext.Vouchers.ToListAsync();
        return _mapper.Map<IEnumerable<VoucherModel>>(vouchers);
    }

    public async Task<VoucherModel> GetVoucherByIdAsync(int id)
    {
        var voucher = await _dbcontext.Vouchers.FindAsync(id);
        return voucher == null ? null : _mapper.Map<VoucherModel>(voucher);
    }

    public async Task<VoucherModel> CreateVoucherAsync(VoucherModel voucherModel)
    {
        var voucher = _mapper.Map<Voucher>(voucherModel);
        await _dbcontext.Vouchers.AddAsync(voucher);
        await _dbcontext.SaveChangesAsync();
        return _mapper.Map<VoucherModel>(voucher);
    }

    public async Task<bool> UpdateVoucherAsync(VoucherModel voucherModel)
    {
        var voucher = _mapper.Map<Voucher>(voucherModel);
        _dbcontext.Vouchers.Update(voucher);
        return await _dbcontext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteVoucherAsync(int id)
    {
        var voucher = await _dbcontext.Vouchers.FindAsync(id);
        if (voucher == null) return false;

        _dbcontext.Vouchers.Remove(voucher);
        return await _dbcontext.SaveChangesAsync() > 0;
    }

    public async Task<bool> IsVoucherCodeUniqueAsync(string voucherCode)
    {
        return !await _dbcontext.Vouchers.AnyAsync(v => v.VoucherCode == voucherCode);
    }
}
