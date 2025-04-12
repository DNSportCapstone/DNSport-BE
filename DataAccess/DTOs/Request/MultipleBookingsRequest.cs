using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Request
{
    public class MultipleBookingsRequest
    {
        public List<FieldBooking> Fields { get; set; }
    }

    public class FieldBooking
    {
        public int FieldId { get; set; }
        public int UserId { get; set; }
        public string FieldName { get; set; }
        public List<SlotBooking> SelectedSlots { get; set; }
    }

    public class SlotBooking
    {
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public bool IsChoose { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public List<ServiceBooking> Services { get; set; }
    }

    public class ServiceBooking
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
