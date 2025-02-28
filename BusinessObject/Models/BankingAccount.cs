using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class BankingAccountModel
{
    public int BankingAccountId { get; set; }

    public int? UserId { get; set; }

    public string? Bank { get; set; }

    public string? Account { get; set; }

    public virtual User? User { get; set; }
}
