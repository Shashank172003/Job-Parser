using Microsoft.AspNetCore.Identity;
using TechYugaAI.Models;

namespace TechYugaAI.Services;

public class AuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // ── REGISTER ──
    public async Task<(bool Success, string Error)> RegisterAsync(string fullName, string email, string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
            return (false, "Email already registered.");

        var user = new AppUser
        {
            FullName = fullName,
            Email = email,
            UserName = email,
            Provider = "Local"
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return (false, result.Errors.First().Description);

        // ← REMOVED SignInAsync from here
        return (true, "");
    }

    // ── LOGIN ──
    public async Task<(bool Success, string Error)> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return (false, "Invalid email or password.");

        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
        if (!result.Succeeded)
            return (false, "Invalid email or password.");

        return (true, "");
    }

    // ── LOGOUT ──
    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}