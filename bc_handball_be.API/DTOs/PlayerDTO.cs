namespace bc_handball_be.API.DTOs
{
    public class PlayerDTO
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int GoalCount { get; set; }
        public int SevenMeterGoalCount { get; set; }
        public int SevenMeterMissCount { get; set; }
        public int TwoMinPenaltyCount { get; set; }
        public int RedCardCount { get; set; }
        public int YellowCardCount { get; set; }
        //public int personId { get; set; }
        public PersonDTO Person { get; set; } = new();
    }
}
