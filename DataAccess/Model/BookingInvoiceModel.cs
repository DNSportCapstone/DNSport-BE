namespace DataAccess.Model
{
    public class BookingInvoiceModel
    {
        public int BookingId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string? StadiumName { get; set; } = string.Empty;
        public string? StadiumAddress { get; set; } = string.Empty;
        public List<InvoiceItem> ItemBooking { get; set; } = new();
        public List<InvoiceItem> ItemService { get; set; } = new();
    }
}
