using OpenAI.Chat;

namespace TechYugaAI.Models;

public class ChatSession
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string Mode { get; set; } = "General";
    public string Title { get; set; } = "New Chat";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public AppUser User { get; set; } = null!;
    public ICollection<UserChatMessage> Messages { get; set; } = new List<UserChatMessage>();
}