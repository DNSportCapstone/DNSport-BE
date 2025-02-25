using System.ComponentModel.DataAnnotations;

namespace DataAccess.Model
{
    public class RatingModel
    {
        public int UserId { get; set; }
        public int BookingId { get; set; }
        [Range(1, 5, ErrorMessage = "RatingValue must be between 1 and 5.")]
        public int? RatingValue { get; set; } 
        public string Comment { get; set; }
    }

}
