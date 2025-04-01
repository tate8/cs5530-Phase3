using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public Course()
        {
            Classes = new HashSet<Class>();
        }

        public uint CourseId { get; set; }
        public string DeptAbrv { get; set; } = null!;
        public uint CNum { get; set; }
        public string CName { get; set; } = null!;

        public virtual Department DeptAbrvNavigation { get; set; } = null!;
        public virtual ICollection<Class> Classes { get; set; }
    }
}
