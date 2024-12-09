using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PROJECT_SEM3.Models;

namespace PROJECT_SEM3.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Các DbSet cho các bảng mới
        public DbSet<Location> Locations { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<UserSpecialization> UserSpecializations { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Gọi cấu hình của Identity

            // Cấu hình quan hệ Many-to-Many giữa Users và Specializations
            builder.Entity<UserSpecialization>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSpecializations)
                .HasForeignKey(us => us.UserId);

            builder.Entity<UserSpecialization>()
                .HasOne(us => us.Specialization)
                .WithMany(s => s.UserSpecializations)
                .HasForeignKey(us => us.SpecializationId);

            // Cấu hình quan hệ Many-to-Many giữa Users (Followers và Followings)
            builder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Follow>()
                .HasOne(f => f.Following)
                .WithMany(u => u.Followings)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ One-to-Many giữa Users và Posts
            builder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId);

            // Cấu hình quan hệ One-to-Many giữa Users và Comments
            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh cascade path

            // Cấu hình quan hệ One-to-Many giữa Posts và Comments
            builder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Chỉ cho phép cascade với Post

            // Cấu hình quan hệ One-to-Many giữa Users (Sender, Receiver) và Messages
            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
