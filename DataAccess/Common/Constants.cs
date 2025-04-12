using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Common
{
    public class Constants
    {
        public enum BookingType
        {
            Booking = 1,
            MultiBooking = 2,
            ScheduleBooking = 3
        }
        public struct BookingStatus
        {
            public const string PendingPayment = "PendingPayment";
            public const string Paid = "Paid";
            public const string Cancelled = "Cancelled";
            public const string Refunded = "Refunded";
        }
    }
}
