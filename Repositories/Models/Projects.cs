using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Projects
    {
        public Projects()
        {
            ProjectShapeConn = new HashSet<ProjectShapeConn>();
            Result = new HashSet<Result>();
        }

        public int Id { get; set; }
        public int MemberId { get; set; }
        public string ProjectName { get; set; }
        public DateTime ProjectDate { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? ProjectStatus { get; set; }

        public virtual Members Member { get; set; }
        public virtual ICollection<ProjectShapeConn> ProjectShapeConn { get; set; }
        public virtual ICollection<Result> Result { get; set; }
    }
}
