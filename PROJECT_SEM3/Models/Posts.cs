namespace PROJECT_SEM3.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } // Foreign Key to Users

        public Users User { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
