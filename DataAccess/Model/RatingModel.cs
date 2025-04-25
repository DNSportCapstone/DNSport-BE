using System.ComponentModel.DataAnnotations;

namespace DataAccess.Model
{
    public class RatingModel
    {
        public int RatingId { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int? RatingValue { get; set; }
        public string Comment { get; set; }
        public DateTime? Time { get; set; }
        public string Reply { get; set; }
        public DateTime? ReplyTime { get; set; }
    }

}
