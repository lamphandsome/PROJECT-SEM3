namespace PROJECT_SEM3.Dtos
{
    public class ChatMessageDTO
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class SendMessageDTO
    {
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public string SenderId { get; set; }
        public string SenderUserName { get; set; }  // Include only necessary sender info
        public string ReceiverId { get; set; }
        public string ReceiverUserName { get; set; }  // Include only necessary receiver info
    }
}
