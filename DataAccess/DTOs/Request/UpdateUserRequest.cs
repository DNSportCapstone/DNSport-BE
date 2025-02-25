﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.Request
{
    public class UpdateUserRequest
    {
        public int UserId { get; set; }
        public int? RoleId { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? RoleName { get; set; }
        public string? Bank { get; set; }
        public string? Account { get; set; }

    }
}
