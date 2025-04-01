﻿using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enrolled
    {
        public uint EId { get; set; }
        public string UId { get; set; } = null!;
        public uint ClassId { get; set; }
        public string Grade { get; set; } = null!;

        public virtual Class Class { get; set; } = null!;
        public virtual Student UIdNavigation { get; set; } = null!;
    }
}
