using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Result
    {
        public int Id { get; set; }
        public decimal PointOfShapeX { get; set; }
        public decimal PointOfShapeY { get; set; }
        public decimal PointOnAreaX { get; set; }
        public decimal PointOnAreaY { get; set; }
        public int ShapeId { get; set; }
        public int ProjectId { get; set; }

        public virtual Projects Project { get; set; }
        public virtual Shapes Shape { get; set; }
    }
}
