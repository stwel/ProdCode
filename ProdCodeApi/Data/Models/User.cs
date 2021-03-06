﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdCodeApi.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime? Birthdate { get; set; }

        public ICollection<UserRole> Roles { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
