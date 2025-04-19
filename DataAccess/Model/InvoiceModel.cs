namespace DataAccess.Model
{
    public class InvoiceModel
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal TotalAmount => CalculateTotalAmount();
        public List<InvoiceItem> Items { get; set; } = new();

        private decimal CalculateTotalAmount()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.TotalPrice;
            }
            return total;
        }
    }

    public class InvoiceItem
    {
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
