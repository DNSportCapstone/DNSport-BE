using Microsoft.AspNetCore.Http;
namespace DataAccess.DTOs.Request
{
    public class ReportRequest
    {
        public int UserId { get; set; }
        public int BookingId { get; set; }
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
    }
}
