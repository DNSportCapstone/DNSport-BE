namespace DataAccess.Model
{
    public class DenounceModel
    {
        public int DenounceId { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? DenounceTime { get; set; }
        public string? UserName { get; set; }
        public DateTime BookingDate { get; set; }
        public string StadiumName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
