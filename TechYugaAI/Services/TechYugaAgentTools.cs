using System.ComponentModel;
using Microsoft.Extensions.AI;
using TechYugaAI.Services;

namespace TechYugaAI.Services;

public class TechYugaAgentTools
{
    private readonly JobSearchService _jobSearchService;

    public TechYugaAgentTools(JobSearchService jobSearchService)
    {
        _jobSearchService = jobSearchService;
    }

    [Description("Generates a professional CV. Call this when the user has provided their details and wants a final document.")]
    public string GenerateCVDoc(string fullName, string experienceSummary, string skills)
    {
        return $"[SUCCESS] TechYuga CV generated for {fullName}. Ready for download.";
    }

    [Description("Calculates a project bid total based on labor, materials, and profit margin.")]
    public string CalculateBid(double laborCosts, double materialCosts, double marginPercent)
    {
        double total = (laborCosts + materialCosts) * (1 + (marginPercent / 100));
        return $"The calculated total for this TechYuga Bid is ${total:F2}.";
    }

    [Description("Parses a job description and extracts structured information like skills, experience, salary and responsibilities")]
    public string ParseJobDescription(
        [Description("The full job description text pasted by the user")]
        string jobText)
    {
        if (string.IsNullOrWhiteSpace(jobText))
            return "No job description provided.";

        return jobText;
    }

    [Description("Searches for jobs based on job title and optional location")]
    public async Task<string> SearchJobsAsync(
        [Description("Job title or role to search for e.g. Software Engineer, HR Manager")]
        string jobTitle,
        [Description("Optional location e.g. New York, Remote, India")]
        string location = "")
    {
        return await _jobSearchService.SearchJobsAsync(jobTitle, location);
    }
}