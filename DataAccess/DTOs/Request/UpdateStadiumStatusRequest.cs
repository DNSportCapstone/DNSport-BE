namespace DataAccess.DTOs.Request
{
    public class UpdateStadiumStatusRequest
    {
        public int StadiumId { get; set; }
        public string NewStatus { get; set; } // "Active" hoặc "Reject"
    }
}
