using BusinessObject.Models;
using DataAccess.Common;
using Net.payOS;

namespace Presentation.BackgroundServices
{
    public class PaymentStatusUpdaterBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PayOS _payOS;
        private const int DelaySeconds = 15;

        public PaymentStatusUpdaterBackgroundService(IServiceProvider serviceProvider, PayOS payOS)
        {
            _serviceProvider = serviceProvider;
            _payOS = payOS;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckPendingPaymentsAsync();
                await Task.Delay(TimeSpan.FromSeconds(DelaySeconds), stoppingToken);
            }
        }
        private async Task CheckPendingPaymentsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Db12353Context>();

            var pendingBookings = db.Bookings
                .Where(b => b.Status == Constants.BookingStatus.PendingPayment).ToList();

            foreach (var booking in pendingBookings)
            {
                try
                {
                    var payment = await _payOS.getPaymentLinkInformation(booking.BookingId);
                    if (payment.status == "CANCELLED" || payment.status == "EXPIRED")
                    {
                        booking.Status = Constants.BookingStatus.Cancelled;
                    }

                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
