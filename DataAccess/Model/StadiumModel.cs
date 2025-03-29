namespace DataAccess.Model
{
    public class StadiumModel
    {
        public int StadiumId { get; set; }

        public int? UserId { get; set; }

        public string? StadiumName { get; set; }

        public string? Address { get; set; }

        public string? Image { get; set; }

        public string? Status { get; set; }
        public string? Owner { get; set; }
    }
}
