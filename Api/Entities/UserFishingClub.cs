namespace Api.Entities
{
    public class UserFishingClub
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int FishingClubId { get; set; }
        public FishingClub FishingClub { get; set; }
    }
}