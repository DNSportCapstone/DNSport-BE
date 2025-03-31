// DataAccess/Repositories/Implement/RefundRepository.cs
using BusinessObject.Models;
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
            var refunds = await _context.Refunds
                .Include(r => r.User)
                    .ThenInclude(u => u.UserDetail)
                .Include(r => r.User)
                    .ThenInclude(u => u.BankingAccounts)
                .Include(r => r.Payment)
                .ToListAsync();

            return refunds.Select(r => new RefundModel
            {
                RefundId = r.RefundId,
                UserId = r.UserId ?? 0,
                PaymentId = r.PaymentId ?? 0,
                RefundAmount = r.RefundAmount ?? 0,
                Status = r.Status,
                Time = r.Time ?? DateTime.MinValue,
                UserName = r.User?.UserDetail?.FullName ?? "",
                BankAccountNumber = r.User?.BankingAccounts.FirstOrDefault()?.Account ?? ""
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

        // DataAccess/Repositories/Implement/RefundRepository.cs
        public async Task<RefundResponseModel> CreateRefundRequestAsync(RefundRequestModel refundRequest)
        {
            var booking = await _context.Bookings
                .Include(b => b.Payments)
                .Include(b => b.BookingFields)
                .FirstOrDefaultAsync(b => b.BookingId == refundRequest.BookingId);

            if (booking == null)
            {
                throw new Exception("Booking không tồn tại");
            }

            if (booking.Status != "Success")
            {
                throw new Exception("Booking chưa được thanh toán hoặc không hợp lệ");
            }

            var latestPayment = booking.Payments?.OrderByDescending(p => p.PaymentTime).FirstOrDefault();
            if (latestPayment == null)
            {
                throw new Exception("Booking chưa có thanh toán để hoàn tiền");
            }

            var existingRefund = await _context.Refunds
                .AnyAsync(r => r.PaymentId == latestPayment.PaymentId && r.Status == "Processing");
            if (existingRefund)
            {
                throw new Exception("Booking này đã có yêu cầu refund đang xử lý");
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
                timeRemainingFormatted = $"{hours} giờ ${minutes} phút";
                timeRemainingInSeconds = timeRemaining.Value.TotalSeconds;
            }

            decimal refundPercentage;
            if (timeRemaining.Value.TotalHours >= 24)
            {
                refundPercentage = 1.0m; // 100%
            }
            else if (timeRemaining.Value.TotalHours >= 12 && timeRemaining.Value.TotalHours < 24)
            {
                refundPercentage = 0.7m; // 70%
            }
            else if (timeRemaining.Value.TotalHours >= 6 && timeRemaining.Value.TotalHours < 12)
            {
                refundPercentage = 0.5m; // 50%
            }
            else // < 6 giờ
            {
                throw new Exception("Không thể yêu cầu refund: Thời gian còn lại dưới 6 giờ, không được hoàn tiền");
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

            return new RefundResponseModel
            {
                RefundId = refund.RefundId,
                RefundAmount = refundAmount,
                TimeRemaining = timeRemainingFormatted,
                TimeRemainingInSeconds = timeRemainingInSeconds,
                RefundPercentage = Math.Round(refundPercentage * 100, 2),
                Bank = refundRequest.Bank
            };
        }

        public async Task<RefundResponseModel> PreviewRefundRequestAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Payments)
                .Include(b => b.BookingFields)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                throw new Exception("Booking không tồn tại");
            }

            if (booking.Status != "Success")
            {
                throw new Exception("Booking chưa được thanh toán hoặc không hợp lệ");
            }

            var latestPayment = booking.Payments?.OrderByDescending(p => p.PaymentTime).FirstOrDefault();
            if (latestPayment == null)
            {
                throw new Exception("Booking chưa có thanh toán để hoàn tiền");
            }

            var existingRefund = await _context.Refunds
                .AnyAsync(r => r.PaymentId == latestPayment.PaymentId && r.Status == "Processing");
            if (existingRefund)
            {
                throw new Exception("Booking này đã có yêu cầu refund đang xử lý");
            }

            if (!booking.BookingFields.Any())
            {
                throw new Exception("Booking không có thông tin sân, không thể tính thời gian hoàn tiền");
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
                timeRemainingInSeconds = timeRemaining.Value.TotalSeconds; // Tính thời gian còn lại bằng giây
            }

            decimal refundPercentage;
            if (timeRemaining.Value.TotalHours < 0)
            {
                throw new Exception("Không thể yêu cầu refund: Thời gian đã hết");
            }
            else if (timeRemaining.Value.TotalHours >= 24)
            {
                refundPercentage = 1.0m; // 100%
            }
            else if (timeRemaining.Value.TotalHours >= 12 && timeRemaining.Value.TotalHours < 24)
            {
                refundPercentage = 0.7m; // 70%
            }
            else if (timeRemaining.Value.TotalHours >= 6 && timeRemaining.Value.TotalHours < 12)
            {
                refundPercentage = 0.5m; // 50%
            }
            else // < 6 giờ
            {
                throw new Exception("Không thể yêu cầu refund: Thời gian còn lại dưới 6 giờ, không được hoàn tiền");
            }

            var totalPrice = booking.TotalPrice ?? 0m;
            var refundAmount = totalPrice * refundPercentage;

            return new RefundResponseModel
            {
                RefundId = 0,
                RefundAmount = refundAmount,
                TimeRemaining = timeRemainingFormatted,
                TimeRemainingInSeconds = timeRemainingInSeconds, 
                RefundPercentage = Math.Round(refundPercentage * 100, 2),
                Bank = "",
            };
        }
        public async Task UpdateRefundStatusAsync(int refundId, string status)
        {
            var refund = await _context.Refunds.FindAsync(refundId);
            if (refund != null)
            {
                if (refund.Status == "Processing" && status == "Completed")
                {
                    refund.Status = status;
                    _context.Refunds.Update(refund);
                }
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