namespace DataAccess.DTOs.Response
{
    public class CreateVoucherResponse
    {
        public int VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
