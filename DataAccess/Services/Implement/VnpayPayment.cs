using Microsoft.Extensions.Configuration;
using VNPAY.NET;

namespace DataAccess.Services.Implement
{
    public class VnpayPayment
    {
        private readonly IVnpay _vnpay;

        public VnpayPayment(IVnpay vnpay, IConfiguration configuration)
        {
            _vnpay = vnpay;
            _vnpay.Initialize(
                configuration["Vnpay:TmnCode"],
                configuration["Vnpay:HashSecret"],
                configuration["Vnpay:Url"],
                configuration["Vnpay:ReturnUrl"]
            );
        }
    }
}
