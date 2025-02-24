namespace DataAccess.Model
{
    public class RatingModel
    {
        public int UserId { get; set; }
        public int? BookingId { get; set; } 
        public int RatingValue { get; set; }
        public string Comment { get; set; }
    }
}
