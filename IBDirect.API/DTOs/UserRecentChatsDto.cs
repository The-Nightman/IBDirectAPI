namespace IBDirect.API.DTOs
{
    public class UserRecentChatsDto
    {
        public string Content { get; set; }
        public DateTime MostRecent { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderRole { get; set; }
    }
}
