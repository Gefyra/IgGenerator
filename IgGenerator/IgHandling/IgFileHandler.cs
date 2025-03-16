using System.Text.RegularExpressions;
using IgGenerator.ConsoleHandling.Interfaces;
using IgGenerator.DataObjectHandling.Interfaces;
using IgGenerator.Helpers;
using IgGenerator.IgHandling.Interfaces;

namespace IgGenerator.IgHandling;

public partial class IgFileHandler : IIgFileHandler
{
    private static class FolderNames
    {
        public const string DataObjects = "Datenobjekte";
        public const string Terminology = "Terminologien";
        public const string Extensions = "Extensions";
        public const string CapabilityStatements = "CapabilityStatements";
    }

    private readonly IUserInteractionHandler _userInteractionHandler;
    private readonly INamingManipulationHandler _namingManipulationHandler;
    private readonly ITemplateHandler _templateHandler;
    private readonly ITocFileManager _tocFileManager;
    private DirectoryInfo _directory;
    public string IgFolderPath { get; private set; }

    public IgFileHandler(
        IUserInteractionHandler userInteractionHandler,
        INamingManipulationHandler namingManipulationHandler,
        ITemplateHandler templateHandler,
        ITocFileManager tocFileManager)
    {
        _userInteractionHandler = userInteractionHandler ?? throw new ArgumentNullException(nameof(userInteractionHandler));
        _namingManipulationHandler = namingManipulationHandler ?? throw new ArgumentNullException(nameof(namingManipulationHandler));
        _templateHandler = templateHandler ?? throw new ArgumentNullException(nameof(templateHandler));
        _tocFileManager = tocFileManager ?? throw new ArgumentNullException(nameof(tocFileManager));
    }

    public void StartConsoleWorkflow()
    {
        IgFolderPath = _userInteractionHandler.GetString("Select IgFolder Path:");
        _directory = new DirectoryInfo(IgFolderPath);
        
        if (!_directory.Exists)
        {
            throw new DirectoryNotFoundException($"Directory not found: {IgFolderPath}");
        }
    }

    public void SaveExtractedDataObjectFiles(IDictionary<string, IDictionary<string, string>> extractedDataObjects)
    {
        string fullPath = GetOrCreateDirectory(FolderNames.DataObjects);

        foreach (var dataObject in extractedDataObjects)
        {
            RemoveEmptyExamples(dataObject);

            string match = _namingManipulationHandler.FilterPartFromFilename(
                LastPartOfCanonical().Match(dataObject.Key).Value);
            string folder = Path.Combine(fullPath, $"Datenobjekt_{match}");
            
            SaveFiles(dataObject.Value, folder);
        }
    }

    private static void RemoveEmptyExamples(KeyValuePair<string, IDictionary<string, string>> dataObject)
    {
        try
        {
            var emptyExample = dataObject.Value.FirstOrDefault(e => 
                e.Key.Contains("Beispiele") && e.Value.Split('\n').Length < 9);
                
            if (!string.IsNullOrEmpty(emptyExample.Key))
            {
                dataObject.Value.Remove(emptyExample.Key);
            }
        }
        catch (InvalidOperationException)
        {
            // Ignore if no empty examples found
        }
    }

    public void SaveExtractedCodeSystemFiles(IDictionary<string, string> extractedCodeSystems)
    {
        string dataObjectPath = GetOrCreateDirectory(FolderNames.DataObjects);
        string fullPath = Path.Combine(dataObjectPath, FolderNames.Terminology);
        EnsureDirectoryExists(fullPath);
        SaveFiles(extractedCodeSystems, fullPath);
    }

    public void SaveExtractedExtensionFiles(IDictionary<string, string> extractedExtensions)
    {
        string dataObjectPath = GetOrCreateDirectory(FolderNames.DataObjects);
        string fullPath = Path.Combine(dataObjectPath, FolderNames.Extensions);
        EnsureDirectoryExists(fullPath);
        SaveFiles(extractedExtensions, fullPath);
    }
    
    public void SaveExtractedCapStmtFiles(IDictionary<string, string> extractedCapStmt)
    {
        string dataObjectPath = GetOrCreateDirectory(FolderNames.DataObjects);
        string fullPath = Path.Combine(dataObjectPath, FolderNames.CapabilityStatements);
        EnsureDirectoryExists(fullPath);
        SaveFiles(extractedCapStmt, fullPath);
    }

    public void SaveCopyPasteFiles()
    {
        string? basePath = _directory.FindFolderPath(FolderNames.DataObjects)?.Replace(FolderNames.DataObjects, "");
        if (basePath != null)
        {
            var templates = _templateHandler.GetTemplate(TemplateType.CopyPasteFile)
                .ToDictionary(e => e.FileName, e => e.Content);
            SaveFiles(templates, basePath);
        }
    }

    public void SaveTocFiles()
    {
        SaveTocFile(FolderNames.DataObjects, _tocFileManager.GetDataObjectTocFile());
        SaveTocFile(FolderNames.Terminology, _tocFileManager.GetCodeSystemTocFile());
        SaveTocFile(FolderNames.Extensions, _tocFileManager.GetExtensionTocFile());
        SaveTocFile(FolderNames.CapabilityStatements, _tocFileManager.GetCapabilitySatementTocFile());
    }

    private void SaveTocFile(string folderName, string content)
    {
        string? folderPath = _directory.FindFolderPath(folderName);
        if (folderPath != null)
        {
            SaveFiles(new Dictionary<string, string>
            {
                {"toc.yaml", content.RemoveEmptyLines()}
            }, folderPath);
        }
    }

    private string GetOrCreateDirectory(string folderName)
    {
        string? existingPath = _directory.FindFolderPath(folderName);
        if (existingPath != null)
        {
            return existingPath;
        }

        string newPath = Path.Combine(_directory.FullName, folderName);
        Directory.CreateDirectory(newPath);
        _userInteractionHandler.Send($"Created directory: {newPath}");
        return newPath;
    }

    private void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            _userInteractionHandler.Send($"Created directory: {path}");
        }
    }

    private void SaveFiles(IDictionary<string, string> files, string fullPath)
    {
        EnsureDirectoryExists(fullPath);

        foreach (var (fileName, content) in files)
        {
            try
            {
                string filePath = Path.Combine(fullPath, fileName);
                File.WriteAllText(filePath, content);
                _userInteractionHandler.Send($"Created file: {filePath}");
            }
            catch (Exception ex)
            {
                _userInteractionHandler.Send($"Error creating file {fileName}: {ex.Message}");
            }
        }
    }

    [GeneratedRegex("[^/]+$")]
    private static partial Regex LastPartOfCanonical();
}