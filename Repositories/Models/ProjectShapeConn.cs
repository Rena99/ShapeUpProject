using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class ProjectShapeConn
    {
        public int ProjectId { get; set; }
        public int ShapeId { get; set; }
        public int Id { get; set; }

        public virtual Projects Project { get; set; }
        public virtual Shapes Shape { get; set; }
    }
}
