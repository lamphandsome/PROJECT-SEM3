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
        public string Thumbnail { get; set; }
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

