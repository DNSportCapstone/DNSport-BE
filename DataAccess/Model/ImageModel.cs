using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class ImageModel
    {
        public int ImageId { get; set; }
        public int FieldId { get; set; }
        public string PublicId { get; set; } // ID lưu trên Cloud (VD: Cloudinary)
        public string Url { get; set; } // Đường dẫn ảnh
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Property
        public Field Field { get; set; }
    }
}
