# TechYugaAI 🚀
### AI-Powered Job Automation Engine

> Automatically search jobs from Naukri.com, parse resumes, and auto-apply to jobs using intelligent browser automation.

---

## 📌 Table of Contents

- [About the Project](#about-the-project)
- [Key Features](#key-features)
- [Tech Stack](#tech-stack)
- [5-Stage Automation Pipeline](#5-stage-automation-pipeline)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [Known Limitations](#known-limitations)
- [Future Enhancements](#future-enhancements)
- [Authors](#authors)

---

## 🧠 About the Project

**TechYugaAI** is a full-stack AI-powered job automation platform built with **Blazor Server (.NET 8)**. It eliminates the manual effort of job searching and applying by automating the entire pipeline — from resume parsing to job application submission.

The platform scrapes live job listings from **Naukri.com** using Selenium WebDriver, matches them to your resume skills, and auto-applies to jobs by filling application forms — including chatbot popups and radio button questions — using a pre-configured Master Data Dictionary.

---

## ✨ Key Features

- 📄 **Resume-Driven Job Search** — Upload PDF resume, skills auto-extracted, jobs searched automatically
- 📃 **Multi-Page Scraping** — Scrapes across multiple Naukri.com pages fetching 15-20+ jobs per session
- 🤖 **Smart Auto-Apply Engine** — Detects and fills CTC, PAN, DOB, Notice Period fields automatically
- 💬 **Chatbot Form Handler** — Handles Naukri Campus AI recruiter chat popup using contenteditable detection
- 🔘 **Radio Button Handler** — Automatically selects correct CTC range and other radio button options
- 🕵️ **Stealth Automation** — User-Agent Spoofing + Chrome flags to bypass bot detection
- 🃏 **Modern Job Cards UI** — Salary, Experience, Location tags with View & Auto Apply buttons
- 👁️ **Visible Browser Mode** — Automation runs in visible Chrome window for live demo
- 💬 **AI Chat Assistant** — GPT-4o Mini powered conversational job assistant
- 📜 **Chat History** — All sessions saved and restored from SQL Server database
- 🔐 **Dual Authentication** — Email/Password login + Google OAuth

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Frontend + Backend | Blazor Server (.NET 8) |
| Browser Automation | Selenium WebDriver + ChromeDriver |
| PDF Parsing | iText7 |
| AI Model | GPT-4o Mini (GitHub Models API) |
| Database | SQL Server Express + Entity Framework Core |
| Authentication | ASP.NET Core Identity + Google OAuth 2.0 |
| Semantic Search | SQLite Vector Store |
| Deployment | Visual Studio Dev Tunnels |
| Stealth Layer | User-Agent Spoofing + Chrome Automation Flags |

---

## ⚙️ 5-Stage Automation Pipeline

```
Stage 1 — Resume Intelligence
  └── Upload PDF → iText7 extracts text → Skills identified automatically

Stage 2 — Job Scraping
  └── Selenium opens Naukri.com → Multi-page scraping → 15-20 jobs fetched

Stage 3 — Job Card Display
  └── Jobs rendered as modern cards → Salary, Experience, Location shown

Stage 4 — Auto-Apply Engine
  └── Bot opens job page → Detects form type → Fills from Master Data
      ├── Normal forms    — XPath field detection + SendKeys
      ├── Chatbot popup   — contenteditable div detection + JS events
      └── Radio buttons   — CTC range matching + label click

Stage 5 — Confirm & Review
  └── Browser stays open → User verifies submission result
```

---

## 🚀 Getting Started

### Prerequisites

- Visual Studio 2022 or later
- .NET 8 SDK
- SQL Server Express
- Chrome Browser (latest version)
- GitHub Models API Token

### Installation

**1. Clone the repository**
```bash
git clone https://github.com/Shashank172003/Job-Parser.git
cd Job-Parser
```

**2. Setup User Secrets**

Right click project → Manage User Secrets → Add:
```json
{
  "GitHubModels": {
    "Token": "your_github_models_token_here"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR-PC-NAME\\SQLEXPRESS;Database=TechYugaAI;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Authentication": {
    "Google": {
      "ClientId": "your_google_client_id",
      "ClientSecret": "your_google_client_secret"
    }
  }
}
```

**3. Run Database Migrations**
```bash
Update-Database
```

**4. Run the Project**
```
Visual Studio → Select "https" profile → Click Run ▶️
```

App opens at: `https://localhost:7062`

---

## ⚙️ Configuration

### Master Data Dictionary
Update your personal details in `Services/JobAutomationService.cs`:

```csharp
private readonly Dictionary<string, string> _masterData = new()
{
    { "current ctc",        "5"             },
    { "expected ctc",       "8"             },
    { "notice period",      "15"            },
    { "current location",   "Delhi"         },
    { "preferred location", "Indore"        },
    { "pan",                "YOUR_PAN_HERE" },
    { "dob",                "DD-MM-YYYY"    },
    { "experience",         "1"             },
    { "qualification",      "MCA"           },
};
```

### Dev Tunnel Setup (for public access URL)
1. Go to **View → Other Windows → Dev Tunnels**
2. Sign in with GitHub account
3. Create tunnel — Name: `TechYugaAI`, Type: `Persistent`, Access: `Public`
4. Run with **"TechYugaAI (Dev Tunnel)"** profile
5. Public URL will appear in Output window

---

## 📁 Project Structure

```
TechYugaAI/
├── Components/
│   └── Pages/
│       ├── Chat.razor                   ← Main page — all modes
│       └── Chat/
│           ├── ChatMessageItem.razor    ← Message rendering
│           ├── JobCards.razor           ← Job cards UI + parsing
│           └── ChatHistorySidebar.razor ← Session sidebar
├── Controllers/
│   └── AccountController.cs            ← Login, Register, OAuth
├── Services/
│   ├── JobSearchService.cs             ← Naukri.com Selenium scraper
│   ├── JobAutomationService.cs         ← Auto-apply bot engine
│   ├── ChatHistoryService.cs           ← Session save and restore
│   ├── FileParserService.cs            ← JD PDF text extraction
│   ├── ResumeParserService.cs          ← Resume PDF skill extraction
│   └── EmailService.cs                ← Email confirmation service
├── Data/
│   └── AppDbContext.cs                 ← EF Core database context
├── Models/
│   └── AppUser.cs                      ← Identity user model
└── Program.cs                          ← All service registrations
```

---

## ⚠️ Known Limitations

- Side-panel form submission not fully consistent on all Naukri job types
- Currently works only on Naukri.com — other portals not yet supported
- CAPTCHA challenges will pause the automation flow
- Google OAuth has a known compatibility issue with Microsoft Edge browser
- Runs locally — not yet deployed to a cloud server

---

## 🔮 Future Enhancements

- [ ] Complete side-panel form submission handling
- [ ] Multi-portal support — LinkedIn, Indeed, Internshala, Shine.com
- [ ] AI Cover Letter Generator per job application
- [ ] Application Tracker Dashboard — Applied, Interview, Rejected, Offered
- [ ] Cloud deployment on Azure App Service
- [ ] Scheduled auto-apply sessions — run at specific times
- [ ] Email confirmation for new user registration
- [ ] Analytics and reporting dashboard

---

## 👨‍💻 Authors

**Shashank Mishra**
- GitHub: [@Shashank172003](https://github.com/Shashank172003)
- Organization: Techpro Compsoft Pvt. Ltd., Noida

**Bhagwan** — Co-developer (Selenium Automation Engine)

---

## 📄 License

This project is built for internal and educational purposes only.
Automated activity on job portals may violate their Terms of Service — use responsibly.

---

<div align="center">
  <strong>Built with ❤️ by Shashank & Bhagwan</strong><br/>
  <em>TechYugaAI — A New Era of AI-Powered Job Automation</em>
</div>
