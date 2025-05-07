using BusinessObject.Models;
using DataAccess.DTOs.Response;
using DataAccess.DTOs;
using DataAccess.Model; // Giả định bạn có entities RevenueTransaction, Booking, BookingField
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.Repositories.Implement
{
    public class RevenueTransactionRepository : IRevenueTransactionRepository
    {
        private readonly Db12353Context _dbcontext; // Thay YourDbContext bằng DbContext của bạn

        public RevenueTransactionRepository(Db12353Context context)
        {
            _dbcontext = context;
        }
        public async Task<List<OwnerAmountResponse>> GetOwnerAmountByFieldIdAsync(OwnerAmountRequest request)
        {
            return await _dbcontext.RevenueTransactions
                .Join(_dbcontext.Bookings,
                    rt => rt.BookingId,
                    b => b.BookingId,
                    (rt, b) => new { RevenueTransaction = rt, Booking = b })
                .Join(_dbcontext.BookingFields,
                    rb => rb.Booking.BookingId,
                    bf => bf.BookingId,
                    (rb, bf) => new { rb.RevenueTransaction, rb.Booking, BookingField = bf })
                .Where(x => x.BookingField.FieldId == request.FieldId)
                .Select(x => new OwnerAmountResponse
                {
                    OwnerAmount = x.RevenueTransaction.OwnerAmount ?? 0, // Sử dụng toán tử ?? để gán 0 nếu null
                    RevenueTransactionDate = x.RevenueTransaction.RevenueTransactionDate // Lấy thêm ngày giao dịch
                })
                .ToListAsync();
        }

    }
}