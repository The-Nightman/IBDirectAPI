namespace IBDirect.API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers =
            new Dictionary<string, List<string>>();

        public Task UserConnected(string userId, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(userId))
                {
                    OnlineUsers[userId].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(userId, new List<string> { connectionId });
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string userId, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(userId))
                    return Task.CompletedTask;

                OnlineUsers[userId].Remove(connectionId);

                if (OnlineUsers[userId].Count == 0)
                {
                    OnlineUsers.Remove(userId);
                }
            }

            return Task.CompletedTask;
        }

        public Task<int[]> GetOnlineUsers()
        {
            int[] onlineUsers;
            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => int.Parse(k.Key)).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        public static Task<string[]> GetConnectionsForUser(string userId)
        {
            string[] connections;
            lock (OnlineUsers)
            {
                connections = OnlineUsers.GetValueOrDefault(userId).ToArray();
            }

            return Task.FromResult(connections);
        }
    }
}
