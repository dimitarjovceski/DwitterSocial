namespace DwitterSocial.Entities
{
    public class UserLikes
    {
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }

        public AppUser LikedUser { get; set; }
        public int LikedUserId { get; set; }
    }   
}
