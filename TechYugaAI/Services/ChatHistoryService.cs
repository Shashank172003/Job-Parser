using Microsoft.EntityFrameworkCore;
using TechYugaAI.Data;
using TechYugaAI.Models;

namespace TechYugaAI.Services;

public class ChatHistoryService
{
    private readonly AppDbContext _db;

    public ChatHistoryService(AppDbContext db)
    {
        _db = db;
    }

    // ── CREATE NEW SESSION ──
    public async Task<ChatSession> CreateSessionAsync(string userId, string mode, string title = "New Chat")
    {
        var session = new ChatSession
        {
            UserId = userId,
            Mode = mode,
            Title = title,
            CreatedAt = DateTime.UtcNow
        };
        _db.ChatSessions.Add(session);
        await _db.SaveChangesAsync();
        return session;
    }

    // ── SAVE MESSAGE ──
    public async Task SaveMessageAsync(int sessionId, string role, string content)
    {
        var message = new UserChatMessage
        {
            ChatSessionId = sessionId,
            Role = role,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };
        _db.UserChatMessages.Add(message);
        await _db.SaveChangesAsync();
    }

    // ── GET ALL SESSIONS FOR USER ──
    public async Task<List<ChatSession>> GetSessionsAsync(string userId)
    {
        return await _db.ChatSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Take(20)
            .ToListAsync();
    }

    // ── GET MESSAGES FOR SESSION ──
    public async Task<List<UserChatMessage>> GetMessagesAsync(int sessionId)
    {
        return await _db.UserChatMessages
            .Where(m => m.ChatSessionId == sessionId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    // ── DELETE SESSION ──
    public async Task DeleteSessionAsync(int sessionId)
    {
        var session = await _db.ChatSessions.FindAsync(sessionId);
        if (session != null)
        {
            _db.ChatSessions.Remove(session);
            await _db.SaveChangesAsync();
        }
    }

    // ── UPDATE SESSION TITLE ──
    public async Task UpdateTitleAsync(int sessionId, string title)
    {
        var session = await _db.ChatSessions.FindAsync(sessionId);
        if (session != null)
        {
            session.Title = title;
            await _db.SaveChangesAsync();
        }
    }
    // ── AUTO GENERATE TITLE FROM FIRST MESSAGE ──
    public async Task UpdateTitleFromMessageAsync(int sessionId, string firstMessage)
    {
        var words = firstMessage.Trim().Split(' ');
        var title = string.Join(" ", words.Take(6));
        if (title.Length > 40) title = title[..40];
        title = title + (words.Length > 6 ? "..." : "");
        await UpdateTitleAsync(sessionId, title);
    }
}