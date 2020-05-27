using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Units
    {
        public Units()
        {
            Shapes = new HashSet<Shapes>();
        }

        public int Id { get; set; }
        public string UnitName { get; set; }

        public virtual ICollection<Shapes> Shapes { get; set; }
    }
}
