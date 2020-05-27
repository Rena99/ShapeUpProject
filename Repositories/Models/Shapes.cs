using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Shapes
    {
        public Shapes()
        {
            Point = new HashSet<Point>();
            ProjectShapeConn = new HashSet<ProjectShapeConn>();
            Result = new HashSet<Result>();
        }

        public int Id { get; set; }
        public bool Area { get; set; }
        public int Unit { get; set; }

        public virtual Units UnitNavigation { get; set; }
        public virtual ICollection<Point> Point { get; set; }
        public virtual ICollection<ProjectShapeConn> ProjectShapeConn { get; set; }
        public virtual ICollection<Result> Result { get; set; }
    }
}
