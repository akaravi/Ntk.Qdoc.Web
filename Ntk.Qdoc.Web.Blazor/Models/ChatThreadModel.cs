using System;
using System.Collections.Generic;
using System.Linq;

namespace Ntk.Qdoc.Web.Blazor.Models
{
    public class ChatThreadModel
    {
        public ChatThreadModel()
        {
            ChatId = Guid.NewGuid().ToString();
            Participants = new List<string>();
            CreatedAt = DateTime.UtcNow;
            LastMessageTime = DateTime.UtcNow;
            UnreadCounts = new Dictionary<string, int>();
        }

        public ChatThreadModel(List<string> participants, string createdBy, string chatName = null)
        {
            ChatId = Guid.NewGuid().ToString();
            Participants = new List<string>(participants ?? new List<string>());
            
            if (!Participants.Contains(createdBy))
            {
                Participants.Add(createdBy);
            }
            
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            LastMessageTime = DateTime.UtcNow;
            ChatName = chatName ?? GenerateChatName(participants);
            UnreadCounts = new Dictionary<string, int>();
            
            // Initialize unread counts for all participants
            foreach (var participant in Participants)
            {
                UnreadCounts[participant] = 0;
            }
        }

        public string ChatId { get; set; }
        public List<string> Participants { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastMessageTime { get; set; }
        public string LastMessagePreview { get; set; }
        public Dictionary<string, int> UnreadCounts { get; set; }
        public string ChatName { get; set; }
        public bool IsGroupChat => Participants.Count > 2;

        public void AddParticipant(string username)
        {
            if (string.IsNullOrEmpty(username))
                return;
                
            if (!Participants.Contains(username))
            {
                Participants.Add(username);
                UnreadCounts[username] = 0;
            }
        }

        public void RemoveParticipant(string username)
        {
            if (Participants.Remove(username))
            {
                UnreadCounts.Remove(username);
            }
        }

        public bool IsParticipant(string username)
        {
            return Participants.Contains(username);
        }

        public int GetUnreadCount(string username)
        {
            return UnreadCounts.TryGetValue(username, out var count) ? count : 0;
        }

        public void IncrementUnreadCount(string username)
        {
            if (UnreadCounts.ContainsKey(username))
            {
                UnreadCounts[username]++;
            }
            else
            {
                UnreadCounts[username] = 1;
            }
        }

        public void ResetUnreadCount(string username)
        {
            if (UnreadCounts.ContainsKey(username))
            {
                UnreadCounts[username] = 0;
            }
        }

        private string GenerateChatName(List<string> participants)
        {
            if (participants == null || participants.Count == 0)
                return "چت جدید";
            
            if (participants.Count <= 3)
            {
                return string.Join(", ", participants);
            }
            
            return $"گروه {participants.Count} نفره";
        }
    }
}
