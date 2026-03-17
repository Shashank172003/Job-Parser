using Microsoft.AspNetCore.Identity;

namespace TechYugaAI.Models;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = "";
    public string Provider { get; set; } = "Local";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
}