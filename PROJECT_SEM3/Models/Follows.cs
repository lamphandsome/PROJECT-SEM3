namespace PROJECT_SEM3.Models
{
    public class Follow
    {
        public int Id { get; set; }
        public string FollowerId { get; set; } // Foreign Key to Users
        public string FollowingId { get; set; } // Foreign Key to Users

        public Users Follower { get; set; }
        public Users Following { get; set; }
    }
}
