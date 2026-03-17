using System.ClientModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;
using TechYugaAI.Components;
using TechYugaAI.Data;
using TechYugaAI.Models;
using TechYugaAI.Services;
using TechYugaAI.Services.Ingestion;

var builder = WebApplication.CreateBuilder(args);

// ── COOKIE POLICY ──
builder.Services.AddCookiePolicy(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// ── DATABASE ──
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── IDENTITY ──
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/login";
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// ── GOOGLE AUTH ──
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
{
    builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
            options.CorrelationCookie.SameSite = SameSiteMode.Lax;
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });
}

// ── CONTROLLERS ──
builder.Services.AddControllersWithViews();

// ── EXISTING SERVICES ──
builder.Services.AddScoped<FileParserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ChatHistoryService>();
builder.Services.AddHttpClient<JobSearchService>();
builder.Services.AddScoped<JobSearchService>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// ── GITHUB MODELS / AI ──
var token = builder.Configuration["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing GitHubModels:Token");
var credential = new ApiKeyCredential(token);
var openAIOptions = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.inference.ai.azure.com")
};
var ghModelsClient = new OpenAIClient(credential, openAIOptions);
var chatClient = ghModelsClient.GetChatClient("gpt-4o-mini").AsIChatClient();
var embeddingGenerator = ghModelsClient.GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();

// ── VECTOR STORE ──
var vectorStorePath = Path.Combine(AppContext.BaseDirectory, "vector-store.db");
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";
builder.Services.AddSqliteVectorStore(_ => vectorStoreConnectionString);
builder.Services.AddSqliteCollection<string, IngestedChunk>(IngestedChunk.CollectionName, vectorStoreConnectionString);
builder.Services.AddSingleton<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();
builder.Services.AddKeyedSingleton("ingestion_directory", new DirectoryInfo(Path.Combine(builder.Environment.WebRootPath, "Data")));

// ── AI CHAT ──
builder.Services.AddChatClient(chatClient)
    .UseFunctionInvocation()
    .UseLogging();
builder.Services.AddEmbeddingGenerator(embeddingGenerator);


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseCookiePolicy();        // ← ADDED
app.UseRouting();             // ← ADDED
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();

// ── GOOGLE LOGIN ENDPOINT ──

app.MapGet("/google-login", async (HttpContext context) =>
{
    var request = context.Request;
    var baseUrl = $"{request.Scheme}://{request.Host}";
    var properties = new AuthenticationProperties
    {
        RedirectUri = $"{baseUrl}/"
    };
    await context.ChallengeAsync("Google", properties);
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();