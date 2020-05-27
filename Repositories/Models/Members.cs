using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Members
    {
        public Members()
        {
            Projects = new HashSet<Projects>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime AccountDate { get; set; }
        public string UserPassword { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Projects> Projects { get; set; }
    }
}
