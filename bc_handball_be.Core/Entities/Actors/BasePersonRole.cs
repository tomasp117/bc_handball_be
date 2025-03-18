using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Entities.IdentityField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Entities.Actors
{
    public abstract class BasePersonRole : BaseEntity
    {
        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;
    }
}
