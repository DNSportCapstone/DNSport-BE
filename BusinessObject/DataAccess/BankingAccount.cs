using System;
using System.Collections.Generic;

namespace BusinessObject.DataAccess;

public partial class BankingAccount
{
    public int BankingAccountId { get; set; }

    public int? UserId { get; set; }

    public string? Bank { get; set; }

    public string? Account { get; set; }

    public virtual User? User { get; set; }
}
