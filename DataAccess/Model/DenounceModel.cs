namespace DataAccess.Model
{
    public class DenounceModel
    {
        public int DenounceId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
