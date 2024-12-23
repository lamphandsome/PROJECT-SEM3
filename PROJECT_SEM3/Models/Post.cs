using System.ComponentModel.DataAnnotations;

namespace PROJECT_SEM3.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string? ImagePath { get; set; } // Không bắt buộc

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? UserId { get; set; }  // Không bắt buộc
        public Users? User { get; set; }  // Không bắt buộc
        public ICollection<Comment>? Comments { get; set; }  // Không bắt buộc
    }
}
