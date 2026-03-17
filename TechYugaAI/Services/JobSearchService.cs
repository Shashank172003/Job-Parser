using System.Text.Json;

namespace TechYugaAI.Services;

public class JobSearchService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public JobSearchService(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    public async Task<string> SearchJobsAsync(string query, string location = "", int page = 1)
    {
        var apiKey = _config["JSearch:ApiKey"];
        var host = _config["JSearch:Host"];

        var searchQuery = string.IsNullOrEmpty(location) ? query : $"{query} in {location}";
        var url = $"https://jsearch.p.rapidapi.com/search?query={Uri.EscapeDataString(searchQuery)}&page={page}&num_pages=1";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiKey);
        _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", host);

        var response = await _httpClient.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<JsonElement>(json);
        var jobs = result.GetProperty("data");

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("```jobcards");

        int count = 0;
        foreach (var job in jobs.EnumerateArray())
        {
            if (count >= 5) break;

            var title = job.GetProperty("job_title").GetString() ?? "";
            var company = job.GetProperty("employer_name").GetString() ?? "";
            var jobCity = job.GetProperty("job_city").GetString() ?? "";
            var jobState = job.GetProperty("job_state").GetString() ?? "";
            var location2 = string.IsNullOrEmpty(jobCity) ? "Remote" : $"{jobCity}, {jobState}";
            var employmentType = job.GetProperty("job_employment_type").GetString() ?? "";
            var applyLink = job.GetProperty("job_apply_link").GetString() ?? "#";
            var description = job.GetProperty("job_description").GetString() ?? "";
            var shortDesc = description.Length > 150 ? description[..150] + "..." : description;

            // Salary
            var minSalary = job.TryGetProperty("job_min_salary", out var minEl) ? minEl.ToString() : "";
            var maxSalary = job.TryGetProperty("job_max_salary", out var maxEl) ? maxEl.ToString() : "";
            var salary = (!string.IsNullOrEmpty(minSalary) && !string.IsNullOrEmpty(maxSalary))
                ? $"{minSalary} - {maxSalary}"
                : "Not disclosed";

            sb.AppendLine($"TITLE:{title}");
            sb.AppendLine($"COMPANY:{company}");
            sb.AppendLine($"LOCATION:{location2}");
            sb.AppendLine($"TYPE:{employmentType}");
            sb.AppendLine($"SALARY:{salary}");
            sb.AppendLine($"DESC:{shortDesc}");
            sb.AppendLine($"LINK:{applyLink}");
            sb.AppendLine("---");
            count++;
        }

        sb.AppendLine("```");
        return sb.ToString();
    }
}