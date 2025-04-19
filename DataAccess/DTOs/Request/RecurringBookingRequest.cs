using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Request
{
    public class RecurringBookingRequest
    {
        public DateTime StartDate { get; set; }
        public int Weekday { get; set; }
        public int RepeatWeeks { get; set; }
        public List<int> ServiceIds { get; set; }
        public int FieldId { get; set; }
        public List<SlotDto> SelectedSlots { get; set; }
        public int UserId { get; set; }
    }


    public class SlotDto
    {
        public string Time { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public bool IsChoose { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public List<int> Services { get; set; }
        public string Date { get; set; }
    }

}
