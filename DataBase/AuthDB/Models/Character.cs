namespace AuthDB.Models
{
    public class Character
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public short Race { get; set; }
        public short Sex { get; set; }
        public short CharacterClass { get; set; }
        public required string CharactertName { get; set; }
        public required DateTime CreateTime { get; set; }

        public Account Account { get; set; } = null!;
    }
}
