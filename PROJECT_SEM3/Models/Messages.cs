namespace PROJECT_SEM3.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; } // Foreign Key to Users
        public string ReceiverId { get; set; } // Foreign Key to Users
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public Users Sender { get; set; }
        public Users Receiver { get; set; }
    }
    class Messages
    {
    }
}
