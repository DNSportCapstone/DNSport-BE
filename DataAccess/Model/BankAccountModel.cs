using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class BankAccountModel
    {
        public int BankingAccountId { get; set; }
        public int? UserId { get; set; }
        public string? Bank { get; set; }
        public string? Account { get; set; }
        public virtual UserModel? User { get; set; }
    }
}
