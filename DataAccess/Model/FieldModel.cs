using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class FieldModel
    {
        public int FieldId { get; set; }
        public string Description { get; set; }
        public decimal DayPrice { get; set; }
        public decimal NightPrice { get; set; }
        public string Status { get; set; }
        public List<ImageModel> Images { get; set; }
        public virtual Stadium Stadium { get; set; }
    }
}
