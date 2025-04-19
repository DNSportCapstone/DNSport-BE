using BusinessObject.Models;

namespace DataAccess.Model
{
    public class FieldModel
    {
        public int FieldId { get; set; }
        public string Description { get; set; }
        public decimal DayPrice { get; set; }
        public decimal NightPrice { get; set; }
        public string Status { get; set; }

        // List of Images
        public List<ImageModel> Images { get; set; }
        public virtual ICollection<BookingFieldModel> BookingFields { get; set; } = new List<BookingFieldModel>();
        public virtual SportModel? Sport { get; set; }
        public virtual StadiumModel? Stadium { get; set; }
    }
}
