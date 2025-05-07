using AutoMapper;
using BusinessObject.Models;
using CloudinaryDotNet;
using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
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
        var existingEntity = await _dbcontext.Vouchers.FindAsync(voucherModel.VoucherId);
        if (existingEntity == null) return false;

        _mapper.Map(voucherModel, existingEntity); // cập nhật vào entity đang được tracked
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

    public async Task<int> CreateOrUpdateVoucher(CreateOrUpdateVoucherRequest model)
    {
        var voucher = await _dbcontext.Vouchers.FirstOrDefaultAsync(v => v.VoucherId == model.VoucherId) ?? new Voucher();
        voucher.VoucherCode = model.VoucherCode;
        voucher.DiscountPercentage = model.DiscountPercentage;
        voucher.ExpiryDate = model.ExpirationDate;

        if (voucher.VoucherId == 0)
        {
            await _dbcontext.Vouchers.AddAsync(voucher);
        }
        await _dbcontext.SaveChangesAsync();
        return voucher.VoucherId;
    }

    public async Task<List<CreateOrUpdateVoucherRequest>> GetAllVouchers()
    {
        var result = await _dbcontext.Vouchers
            .Select(v => new CreateOrUpdateVoucherRequest
            {
                VoucherId = v.VoucherId,
                VoucherCode = v.VoucherCode,
                DiscountPercentage = (int)v.DiscountPercentage,
                ExpirationDate = (DateTime)v.ExpiryDate
            })
            .ToListAsync();
        return result;
    }
    public async Task<VoucherResponse> ApplyVoucher(string voucherCode)
    {
        try
        {
            if (!string.IsNullOrEmpty(voucherCode))
            {
                var voucher = _dbcontext.Vouchers.FirstOrDefault(vc => voucherCode.Equals(vc.VoucherCode));
                if (voucher == null)
                {
                    return new VoucherResponse
                    {
                        IsError = true,
                        Message = "Voucher Code is not valid!"
                    };
                }
                else
                {
                    if (voucher.ExpiryDate <= DateTime.UtcNow.AddHours(7))
                    {
                        return new VoucherResponse
                        {
                            IsError = true,
                            Message = "Voucher Code is expired!"
                        };
                    }
                    else
                    {
                        return new VoucherResponse
                        {
                            IsError = false,
                            Message = "Voucher Code is valid!",
                            VoucherId = voucher.VoucherId,
                            VoucherCode = voucher.VoucherCode,
                            DiscountPercentage = (double)(voucher.DiscountPercentage ?? 0),
                            ExpiryDate = voucher.ExpiryDate ?? DateTime.UtcNow,
                            Conditions = string.Empty
                        };
                    }
                }
            }

            return new VoucherResponse
            {
                IsError = false,
                Message = "Voucher Code is not valid!"
            };
        }
        catch
        {
            return new VoucherResponse
            {
                IsError = true,
                Message = "Apply Voucher Error!"
            };
        }
    }
}
