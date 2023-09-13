﻿using Domain.Entities;

namespace Domain.Entities
{
    public class CourseStudent
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
