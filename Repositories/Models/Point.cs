using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Point
    {
        public int Id { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public int ShapeId { get; set; }

        public virtual Shapes Shape { get; set; }
    }
}
