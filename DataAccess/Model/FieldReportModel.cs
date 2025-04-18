namespace DataAccess.Model
{
    public class FieldReportModel
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string StadiumName { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public int ViolationCount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
