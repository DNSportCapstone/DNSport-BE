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
        private readonly Db12353Context _context; // Thay YourDbContext bằng DbContext của bạn

        public RevenueTransactionRepository(Db12353Context context)
        {
            _context = context;
        }

        public async Task<List<OwnerAmountResponse>> GetOwnerAmountByFieldIdAsync(OwnerAmountRequest request)
        {
            return await _context.RevenueTransactions
                .Join(_context.Bookings,
                    rt => rt.BookingId,
                    b => b.BookingId,
                    (rt, b) => new { RevenueTransaction = rt, Booking = b })
                .Join(_context.BookingFields,
                    rb => rb.Booking.BookingId,
                    bf => bf.BookingId,
                    (rb, bf) => new { rb.RevenueTransaction, rb.Booking, BookingField = bf })
                .Where(x => x.BookingField.FieldId == request.FieldId)
                .Select(x => new OwnerAmountResponse
                {
                    OwnerAmount = x.RevenueTransaction.OwnerAmount ?? 0 // Sử dụng toán tử ?? để gán 0 nếu null
                })
                .ToListAsync();
        }
    }
}