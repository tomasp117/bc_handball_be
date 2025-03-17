namespace bc_handball_be.Core.Entities.Actors.sub
{
    public class Coach : super.Person
    {
        public int? PlayerVoteId { get; set; }
        public int? GoalkeeperVoteId { get; set; }
        public char License { get; set; }

        // Foreign keys
        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
