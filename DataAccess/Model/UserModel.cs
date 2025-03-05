﻿using BusinessObject.Models;

namespace DataAccess.Model
{
    public class UserModel
    {
        public int UserId { get; set; }
        public int? RoleId { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public virtual UserDetailModel? UserDetail { get; set; }
        public virtual ICollection<BankingAccount> BankingAccounts { get; set; } = new List<BankingAccount>();
    }
}
