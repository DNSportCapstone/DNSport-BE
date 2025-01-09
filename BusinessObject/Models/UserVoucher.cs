using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class UserVoucher
{
    public int UserVoucherId { get; set; }

    public int? UserId { get; set; }

    public int? VoucherId { get; set; }

    public bool? IsUsed { get; set; }

    public virtual User? User { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
