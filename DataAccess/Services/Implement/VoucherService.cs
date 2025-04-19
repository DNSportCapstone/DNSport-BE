using AutoMapper;
using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class VoucherService : IVoucherService
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IMapper _mapper;

    public VoucherService(IVoucherRepository voucherRepository, IMapper mapper)
    {
        _voucherRepository = voucherRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VoucherResponse>> GetAllVouchersAsync()
    {
        var vouchers = await _voucherRepository.GetAllVouchersAsync();
        return _mapper.Map<IEnumerable<VoucherResponse>>(vouchers);
    }

    public async Task<CreateVoucherResponse> CreateVoucherAsync(CreateVoucherRequest request)
    {
        bool isUnique = await _voucherRepository.IsVoucherCodeUniqueAsync(request.VoucherCode);
        if (!isUnique)
            throw new Exception("Voucher code already exists.");

        var voucherModel = _mapper.Map<VoucherModel>(request);
        var createdVoucher = await _voucherRepository.CreateVoucherAsync(voucherModel);
        return _mapper.Map<CreateVoucherResponse>(createdVoucher);
    }

    public async Task<VoucherResponse> GetVoucherByIdAsync(int id)
    {
        var voucher = await _voucherRepository.GetVoucherByIdAsync(id);
        return voucher == null ? null : _mapper.Map<VoucherResponse>(voucher);
    }

    public async Task<VoucherResponse> UpdateVoucherAsync(int id, UpdateVoucherRequest request)
    {
        var existingVoucher = await _voucherRepository.GetVoucherByIdAsync(id);
        if (existingVoucher == null)
            throw new Exception("Voucher not found.");

        _mapper.Map(request, existingVoucher);
        await _voucherRepository.UpdateVoucherAsync(existingVoucher);

        return _mapper.Map<VoucherResponse>(existingVoucher);
    }

    public async Task<bool> DeleteVoucherAsync(int id)
    {
        return await _voucherRepository.DeleteVoucherAsync(id);
    }
    public async Task<bool> IsVoucherCodeUniqueAsync(string voucherCode)
    {
        return await _voucherRepository.IsVoucherCodeUniqueAsync(voucherCode);
    }

    public async Task<int> CreateOrUpdateVoucher(CreateOrUpdateVoucherRequest request)
    {
        return await _voucherRepository.CreateOrUpdateVoucher(request);
    }

    public async Task<List<CreateOrUpdateVoucherRequest>> GetAllVouchers()
    {
        return await _voucherRepository.GetAllVouchers();
    }
}
