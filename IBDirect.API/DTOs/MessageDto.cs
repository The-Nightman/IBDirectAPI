namespace IBDirect.API.DTOs
{
    public class MessageDto
    {
        public string Content { get; set; }
        public DateTime DateSent { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderRole { get; set; }
        public int RecipientId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientRole { get; set; }
        public bool Read { get; set; }
    }
}
