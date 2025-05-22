using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Entities.IdentityField;

namespace bc_handball_be.Core.Entities.Actors.sub
{
    public class Player : BasePersonRole
    {
        public int Number { get; set; }
        //public string FirstName { get; set; } = string.Empty;
        //public string LastName { get; set; } = string.Empty;
        public int GoalCount { get; set; } = 0;
        public int SevenMeterGoalCount { get; set; } = 0;
        public int SevenMeterMissCount { get; set; } = 0;
        public int TwoMinPenaltyCount { get; set; } = 0;
        public int RedCardCount { get; set; } = 0;
        public int YellowCardCount { get; set; } = 0;

        // Foreign keys
        public int? TeamId { get; set; }
        public Team? Team { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

    }
}
