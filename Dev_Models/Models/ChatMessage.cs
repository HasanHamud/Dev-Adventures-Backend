namespace Dev_Models.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }

    }
}
