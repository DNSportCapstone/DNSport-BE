using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class User
{
    public int UserId { get; set; }

    public int? RoleId { get; set; }

    public string? Email { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<BankingAccountModel> BankingAccounts { get; set; } = new List<BankingAccountModel>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Denounce> DenounceReceives { get; set; } = new List<Denounce>();

    public virtual ICollection<Denounce> DenounceSends { get; set; } = new List<Denounce>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Stadium> Stadia { get; set; } = new List<Stadium>();

    public virtual ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();

    public virtual UserDetail? UserDetail { get; set; }

    public virtual ICollection<UserVoucher> UserVouchers { get; set; } = new List<UserVoucher>();
}
