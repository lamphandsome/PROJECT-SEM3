namespace PROJECT_SEM3.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int PostId { get; set; } // Foreign Key to Posts
        public string UserId { get; set; } // Foreign Key to Users

        public Post Post { get; set; }
        public Users User { get; set; }
    }
}
