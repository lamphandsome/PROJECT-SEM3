using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace PROJECT_SEM3.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? LocationId { get; set; } // Nullable Foreign Key
        public int YearsOfExperience { get; set; }
<<<<<<< HEAD
        public string? Thumbnail { get; set; }
=======
        public string Thumbnail { get; set; } = "default-thumbnail-url";
>>>>>>> 6e9dda22da4cd58c213a23a7cba6cfadd0be5f93
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Location Location { get; set; }
        public ICollection<UserSpecialization> UserSpecializations { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Follow> Followers { get; set; }
        public ICollection<Follow> Followings { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }
    }
}

