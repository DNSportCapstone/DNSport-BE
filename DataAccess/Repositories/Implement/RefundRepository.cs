using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implement
{
    public class RefundRepository : IRefundRepository
    {
        private readonly Db12353Context _context;

        public RefundRepository(Db12353Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RefundModel>> GetAllRefundsAsync()
        {

            var refunds = await _context.Refunds.AsNoTracking()
                .Include(r => r.User)
                    .ThenInclude(u => u.UserDetail)
                .Include(r => r.User)
                    .ThenInclude(u => u.BankingAccounts)
                .Include(r => r.Payment)
                    .ThenInclude(p => p.Booking)
                .ToListAsync();

            return refunds.Select(r => new RefundModel
            {
                RefundId = r.RefundId,
                BookingId = r.Payment?.Booking?.BookingId,
                UserId = r.UserId ?? 0,
                PaymentId = r.PaymentId ?? 0,
                RefundAmount = r.RefundAmount ?? 0,
                Status = r.Status,
                Time = r.Time ?? DateTime.MinValue,
                UserName = r.User?.UserDetail?.FullName ?? "",
                UserEmail = r.User?.Email,
                BankAccountNumber = r.User?.BankingAccounts.FirstOrDefault()?.Account ?? "",
                BookingDate = r.Payment?.Booking?.BookingDate,
                TotalAmount = r.Payment?.Booking?.TotalPrice
            });
        }

        public async Task<RefundModel> GetRefundByIdAsync(int refundId)
        {
            var refund = await _context.Refunds
                .Include(r => r.User)
                    .ThenInclude(u => u.UserDetail)
                .Include(r => r.User)
                    .ThenInclude(u => u.BankingAccounts)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.RefundId == refundId);

            if (refund == null) return null;

            return new RefundModel
            {
                RefundId = refund.RefundId,
                UserId = refund.UserId ?? 0,
                PaymentId = refund.PaymentId ?? 0,
                RefundAmount = refund.RefundAmount ?? 0,
                Status = refund.Status,
                Time = refund.Time ?? DateTime.MinValue,
                UserName = refund.User?.UserDetail?.FullName ?? "",
                UserEmail = refund.User?.Email,
                BankAccountNumber = refund.User?.BankingAccounts.FirstOrDefault()?.Account ?? ""
            };
        }

        public async Task<bool> CanRequestRefundAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingFields)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null || booking.Status != "Success")
            {
                return false;
            }

            var earliestStartTime = booking.BookingFields
                .Min(bf => bf.StartTime);

            return DateTime.Now < earliestStartTime;
        }

        public async Task<RefundResponseModel> CreateRefundRequestAsync(RefundRequestModel refundRequest)
        {
            var booking = await _context.Bookings
                .Include(b => b.Payments)
                .Include(b => b.BookingFields)
                .FirstOrDefaultAsync(b => b.BookingId == refundRequest.BookingId);

            var response = new RefundResponseModel();

            if (booking == null)
            {
                response.Error = "NOT_FOUND";
                response.Message = "Booking không tồn tại";
                return response;
            }

            if (booking.Status != "Success")
            {
                response.Error = "INVALID";
                response.Message = "Booking chưa được thanh toán hoặc không hợp lệ";
                return response;
            }

            var latestPayment = booking.Payments?.OrderByDescending(p => p.PaymentTime).FirstOrDefault();
            if (latestPayment == null)
            {
                response.Error = "NO_PAYMENT";
                response.Message = "Booking chưa có thanh toán để hoàn tiền";
                return response;
            }

            var existingRefund = await _context.Refunds
                .AnyAsync(r => r.PaymentId == latestPayment.PaymentId && r.Status == "Processing");
            if (existingRefund)
            {
                response.Error = "DUPLICATE";
                response.Message = "Booking này đã có yêu cầu refund đang xử lý";
                return response;
            }

            var earliestStartTime = booking.BookingFields.Min(bf => bf.StartTime);
            var timeRemaining = earliestStartTime - DateTime.Now;
            string timeRemainingFormatted;
            double timeRemainingInSeconds;
            if (!timeRemaining.HasValue || timeRemaining.Value.TotalHours < 0)
            {
                timeRemainingFormatted = "Hết thời gian";
                timeRemainingInSeconds = 0;
            }
            else
            {
                var hours = (int)timeRemaining.Value.TotalHours;
                var minutes = (int)timeRemaining.Value.Minutes;
                timeRemainingFormatted = $"{hours} giờ {minutes} phút";
                timeRemainingInSeconds = timeRemaining.Value.TotalSeconds;
            }

            decimal refundPercentage;
            if (timeRemaining.Value.TotalHours >= 24)
            {
                refundPercentage = 1.0m;
            }
            else if (timeRemaining.Value.TotalHours >= 12 && timeRemaining.Value.TotalHours < 24)
            {
                refundPercentage = 0.7m;
            }
            else if (timeRemaining.Value.TotalHours >= 6 && timeRemaining.Value.TotalHours < 12)
            {
                refundPercentage = 0.5m;
            }
            else
            {
                response.Error = "TIME_EXPIRED";
                response.Message = "Không thể yêu cầu refund: Thời gian còn lại dưới 6 giờ";
                return response;
            }

            var totalPrice = booking.TotalPrice ?? 0m;
            var refundAmount = totalPrice * refundPercentage;

            var refund = new Refund
            {
                UserId = booking.UserId,
                PaymentId = latestPayment.PaymentId,
                RefundAmount = refundAmount,
                Status = "Processing",
                Time = DateTime.Now
            };

            var bankingAccount = await _context.BankingAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == booking.UserId);

            if (bankingAccount == null)
            {
                bankingAccount = new BankingAccount
                {
                    UserId = booking.UserId,
                    Account = refundRequest.BankAccountNumber,
                    Bank = refundRequest.Bank
                };
                _context.BankingAccounts.Add(bankingAccount);
            }
            else
            {
                bankingAccount.Account = refundRequest.BankAccountNumber;
                bankingAccount.Bank = refundRequest.Bank;
                _context.BankingAccounts.Update(bankingAccount);
            }

            await _context.Refunds.AddAsync(refund);
            await _context.SaveChangesAsync();

            response.RefundId = refund.RefundId;
            response.RefundAmount = refundAmount;
            response.TimeRemaining = timeRemainingFormatted;
            response.TimeRemainingInSeconds = timeRemainingInSeconds;
            response.RefundPercentage = Math.Round(refundPercentage * 100, 2);
            response.Bank = refundRequest.Bank;
            response.Message = "Refund request submitted successfully";
            response.Error = null;

            return response;
        }

        public async Task<RefundResponseModel> PreviewRefundRequestAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Payments)
                .Include(b => b.BookingFields)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            var response = new RefundResponseModel();

            if (booking == null)
            {
                response.Error = "NOT_FOUND";
                response.Message = "Booking không tồn tại";
                return response;
            }

            if (booking.Status != "Success")
            {
                response.Error = "INVALID";
                response.Message = "Booking chưa được thanh toán hoặc không hợp lệ";
                return response;
            }

            var latestPayment = booking.Payments?.OrderByDescending(p => p.PaymentTime).FirstOrDefault();
            if (latestPayment == null)
            {
                response.Error = "NO_PAYMENT";
                response.Message = "Booking chưa có thanh toán để hoàn tiền";
                return response;
            }

            var existingRefund = await _context.Refunds
                .AnyAsync(r => r.PaymentId == latestPayment.PaymentId && r.Status == "Processing");
            if (existingRefund)
            {
                response.Error = "DUPLICATE";
                response.Message = "Booking này đã có yêu cầu refund đang xử lý";
                return response;
            }

            if (!booking.BookingFields.Any())
            {
                response.Error = "NO_FIELDS";
                response.Message = "Booking không có thông tin sân";
                return response;
            }

            var earliestStartTime = booking.BookingFields.Min(bf => bf.StartTime);
            var timeRemaining = earliestStartTime - DateTime.Now;

            string timeRemainingFormatted;
            double timeRemainingInSeconds;
            if (timeRemaining.Value.TotalHours < 0)
            {
                timeRemainingFormatted = "Hết thời gian";
                timeRemainingInSeconds = 0;
            }
            else
            {
                var hours = (int)timeRemaining.Value.TotalHours;
                var minutes = (int)timeRemaining.Value.Minutes;
                timeRemainingFormatted = $"{hours} giờ {minutes} phút";
                timeRemainingInSeconds = timeRemaining.Value.TotalSeconds;
            }

            decimal refundPercentage;
            if (timeRemaining.Value.TotalHours < 0)
            {
                response.Error = "TIME_EXPIRED";
                response.Message = "Không thể yêu cầu refund: Thời gian đã hết";
                return response;
            }
            else if (timeRemaining.Value.TotalHours >= 24)
            {
                refundPercentage = 1.0m;
            }
            else if (timeRemaining.Value.TotalHours >= 12 && timeRemaining.Value.TotalHours < 24)
            {
                refundPercentage = 0.7m;
            }
            else if (timeRemaining.Value.TotalHours >= 6 && timeRemaining.Value.TotalHours < 12)
            {
                refundPercentage = 0.5m;
            }
            else
            {
                response.Error = "TIME_EXPIRED";
                response.Message = "Không thể yêu cầu refund: Thời gian còn lại dưới 6 giờ";
                return response;
            }

            var totalPrice = booking.TotalPrice ?? 0m;
            var refundAmount = totalPrice * refundPercentage;

            response.RefundId = 0;
            response.RefundAmount = refundAmount;
            response.TimeRemaining = timeRemainingFormatted;
            response.TimeRemainingInSeconds = timeRemainingInSeconds;
            response.RefundPercentage = Math.Round(refundPercentage * 100, 2);
            response.Bank = "";
            response.Message = "Preview refund successful";
            response.Error = null;

            return response;
        }

        public async Task UpdateRefundStatusAsync(int refundId, string status)
        {
            var refund = await _context.Refunds.FindAsync(refundId);
            if (refund != null && refund.Status == "Processing" && status == "Completed")
            {
                refund.Status = status;
                _context.Refunds.Update(refund);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task DeleteRefundAsync(int refundId)
        {
            var refund = await _context.Refunds.FindAsync(refundId);
            if (refund != null)
            {
                _context.Refunds.Remove(refund);
            }
        }
    }
}