namespace IBDirect.API.DTOs
{
    public class UserInboxDto
    {
        public List<UserUnreadChatsDto> UnreadChats { get; set; }
        public List<UserRecentChatsDto> RecentChats { get; set; }
    }
}
