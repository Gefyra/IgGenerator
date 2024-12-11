using System.IO.Compression;
using System.Text;
using System.Text.Json;
using IgGenerator.ConsoleHandling.Interfaces;
using IgGenerator.Helpers;

namespace IgGenerator.Simplifier;

public class SimplifierConnector(IUserInteractionHandler userInteractionHandler) 
    : ISimplifierConnector
{

    private string GetProjectUrl()
    {
         return userInteractionHandler.GetString("Simplifier Project Url:");
    }

    public void LoadTemplate()
    {
        DownloadZipAsync(GetProjectUrl()).Wait();

        if (Directory.Exists("Project"))
        {
            Directory.Delete("Project", true);
        }
        ZipFile.ExtractToDirectory("./Project.zip", "Project");
        
        DirectoryInfo directory = new("Project");
        DirectoryInfo? guides = directory.FindFolderPathDirectoryInfo("guides");
        if (guides is not null && guides.EnumerateDirectories().Count(d=>d.Name.Contains("Template")) == 1)
        {
            foreach (FileInfo file in guides
                         .EnumerateDirectories()
                         .First(d=>d.Name.Contains("Template"))
                         .EnumerateFiles("*.md", SearchOption.AllDirectories))
            {
                file.MoveTo(file.FullName.Replace("__", "$$"));
            }
            
            foreach (FileInfo file in guides
                         .EnumerateDirectories()
                         .First(d=>d.Name.Contains("Template"))
                         .EnumerateFiles("$$toc.yaml.page.md", SearchOption.AllDirectories))
            {
                file.MoveTo(file.FullName.Replace("$$toc.yaml.page.md", "toc.yaml"), true);
            }
            
            Directory.Delete("./IgTemplate", true);
            guides
                .EnumerateDirectories()
                .First(d=>d.Name.Contains("Template"))
                .MoveTo("./IgTemplate");
        }
        
        
    }

    private async Task DownloadZipAsync(string projectUrl)
    {
        try
        {
            string? token = await GetBearerTokenAsync();

            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using HttpResponseMessage response = await httpClient.GetAsync(projectUrl);

            if (response.IsSuccessStatusCode)
            {
                await using Stream responseStream = await response.Content.ReadAsStreamAsync();
                await using FileStream fileStream = new("./Project.zip", FileMode.Create, FileAccess.Write);
                await responseStream.CopyToAsync(fileStream);

                Console.WriteLine("ZIP file downloaded successfully to 'Project.zip'");
            }
            else
            {
                Console.WriteLine($"Failed to download ZIP file. Status Code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    private async Task<string?> GetBearerTokenAsync()
    {
        try
        {
            using HttpClient httpClient = new();

            var requestBody = new
            {
                Email = userInteractionHandler.GetString("Simplifier Username:"),
                Password = userInteractionHandler.GetString("Simplifier Password:")
            };

            string json = JsonSerializer.Serialize(requestBody);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await httpClient.PostAsync("https://api.simplifier.net/token", content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                using JsonDocument document = JsonDocument.Parse(responseBody);
                return document.RootElement.GetProperty("token").GetString();
            }
            else
            {
                throw new Exception($"Failed to retrieve token. Status Code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while fetching the token: {ex.Message}");
        }
    }   
}

public interface ISimplifierConnector
{
    public void LoadTemplate();
}