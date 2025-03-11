using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class RatingReplyModel
    {
        public int RatingId { get; set; }

        [Required(ErrorMessage = "Reply cannot be empty.")]
        public string Reply { get; set; }
    }
}
