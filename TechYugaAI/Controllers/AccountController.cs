using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechYugaAI.Models;

namespace TechYugaAI.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Register([FromForm] string fullName, [FromForm] string email, [FromForm] string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName))
            return Redirect("/register?error=Please fill in all fields");

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
            return Redirect("/register?error=Email already registered");

        var user = new AppUser
        {
            FullName = fullName,
            Email = email,
            UserName = email,
            Provider = "Local"
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return Redirect($"/register?error={result.Errors.First().Description}");

        await _signInManager.SignInAsync(user, isPersistent: false);
        return Redirect("/");
    }

    [HttpPost("login")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            return Redirect("/login?error=Please fill in all fields");

        var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
        if (!result.Succeeded)
            return Redirect("/login?error=Invalid email or password");

        return Redirect("/");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Redirect("/");
    }
}