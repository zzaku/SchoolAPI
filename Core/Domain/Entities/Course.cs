﻿using JwtApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public ICollection<User> Users { get; set; }
    }
}
