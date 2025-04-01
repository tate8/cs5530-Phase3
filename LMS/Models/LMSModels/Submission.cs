using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public uint SubmissionId { get; set; }
        public uint AssignId { get; set; }
        public string UId { get; set; } = null!;
        public string Answer { get; set; } = null!;
        public uint? Score { get; set; }
        public DateTime SubmitTime { get; set; }

        public virtual Assignment Assign { get; set; } = null!;
        public virtual Student UIdNavigation { get; set; } = null!;
    }
}
