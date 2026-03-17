using System.ComponentModel;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace TechYugaAI.Services;

public class FileParserService
{
    [Description("Extracts text from a PDF file uploaded by the user")]
    public async Task<string> ExtractFromPdfAsync(Stream pdfStream)
    {
        var textBuilder = new System.Text.StringBuilder();

        // ← KEY FIX: Copy to MemoryStream first (Blazor streams are async only)
        using var memoryStream = new MemoryStream();
        await pdfStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        await Task.Run(() =>
        {
            using var pdf = PdfDocument.Open(memoryStream);
            foreach (Page page in pdf.GetPages())
            {
                textBuilder.AppendLine(page.Text);
            }
        });

        return textBuilder.ToString();
    }
}