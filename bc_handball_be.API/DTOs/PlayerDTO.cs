namespace bc_handball_be.API.DTOs
{
    public class PlayerDTO
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int GoalCount { get; set; }
        public int SevenMeterGoalCount { get; set; }
        public int SevenMeterMissCount { get; set; }
        public int TwoMinPenaltyCount { get; set; }
        public int RedCardCount { get; set; }
        public int YellowCardCount { get; set; }
    }
}
