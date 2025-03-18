using bc_handball_be.Core.Entities.IdentityField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Entities.Actors.sub
{
    public class ClubAdmin : BasePersonRole
    {
        public int ClubId { get; set; }
        public Club Club { get; set; } = null!;
    }
}
