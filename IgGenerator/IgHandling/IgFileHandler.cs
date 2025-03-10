using System.Text.RegularExpressions;
using IgGenerator.ConsoleHandling.Interfaces;
using IgGenerator.DataObjectHandling.Interfaces;
using IgGenerator.Helpers;
using IgGenerator.IgHandling.Interfaces;

namespace IgGenerator.IgHandling;

public partial class IgFileHandler :IIgFileHandler
{
    private readonly IUserInteractionHandler _userInteractionHandler;
    private readonly INamingManipulationHandler _namingManipulationHandler;
    private DirectoryInfo _directory;
    private readonly ITemplateHandler _templateHandler;
    private readonly ITocFileManager _tocFileManager;
    public string igFolderPath { get; private set; }

    public IgFileHandler(IUserInteractionHandler userInteractionHandler, INamingManipulationHandler namingManipulationHandler,ITemplateHandler templateHandler, ITocFileManager tocFileManager)
    {
        _userInteractionHandler = userInteractionHandler;
        _namingManipulationHandler = namingManipulationHandler;
        _templateHandler = templateHandler;
        _tocFileManager = tocFileManager;
    }

    public void StartConsoleWorkflow()
    {
        igFolderPath = _userInteractionHandler.GetString("Select IgFolder Path:");
        _directory = new DirectoryInfo(igFolderPath);
    }

    public void SaveExtractedDataObjectFiles(IDictionary<string, IDictionary<string, string>> extractedDataObjects)
    {
        string dataObjectFolderName = "Datenobjekte";
        string? fullPath = _directory.FindFolderPath(dataObjectFolderName);

        if (fullPath == null)
        {
            fullPath = _directory.CreateSubdirectory($"{dataObjectFolderName}").FullName;
            _userInteractionHandler.Send($"There is no subfolder {dataObjectFolderName} at {_directory.FullName}. Folder created!");
        }
        
        foreach (KeyValuePair<string, IDictionary<string, string>> dataObject in extractedDataObjects)
        {
            RemoveEmptyExamples(dataObject);

            string match = _namingManipulationHandler.FilterPartFromFilename(LastPartOfCanonical().Match(dataObject.Key).Value);
            string folder = $"{fullPath}/Datenobjekt_{match}";
            SimpleAllFilesFromDirectory(dataObject.Value, folder);
        }
    }

    private static void RemoveEmptyExamples(KeyValuePair<string, IDictionary<string, string>> dataObject)
    {
        try
        {
            KeyValuePair<string, string> emptyExample = dataObject.Value.First(e => e.Key.Contains("Beispiele")
                && e.Value.Split('\n').Length < 9);
            dataObject.Value.Remove(emptyExample);
        }
        catch (InvalidOperationException)
        {
            return;
        }
    }

    public void SaveExtractedCodeSystemFiles(IDictionary<string, string> extractedCodeSystems)
    {
        const string terminologyFolderName = "Terminologien";
        string? fullPath = GetDataObjectPath(terminologyFolderName);

        SimpleAllFilesFromDirectory(extractedCodeSystems, fullPath);
    }

    public void SaveExtractedExtensionFiles(IDictionary<string, string> extractedExtensions)
    {
        const string extensionFolderName = "Extensions";
        string? fullPath = GetDataObjectPath(extensionFolderName);

        SimpleAllFilesFromDirectory(extractedExtensions, fullPath);
    }
    
    public void SaveExtractedCapStmtFiles(IDictionary<string, string> extractedCapStmt)
    {
        const string capStmtFolderName = "CapabilityStatements";
        string? fullPath = GetDataObjectPath(capStmtFolderName);

        SimpleAllFilesFromDirectory(extractedCapStmt, fullPath);
    }

    public void SaveCopyPasteFiles()
    {
        string dataObjectFolderName = "Datenobjekte";
        string? fullPath = _directory.FindFolderPath(dataObjectFolderName)?.Replace(dataObjectFolderName, "");
        SimpleAllFilesFromDirectory(_templateHandler.GetTemplate(TemplateType.CopyPasteFile).ToDictionary(e=>e.FileName, e=>e.Content), fullPath);
    }

    public void SaveTocFiles()
    {
        SaveDataObjectTocFile();
        SaveCodesystemTocFile();
        SaveExtensionTocFile();
        SaveCapabilityStatementTocFile();
    }

    private void SaveDataObjectTocFile()
    {
        string? tocFileFolderPath = _directory.FindFolderPath("Datenobjekte");
        SimpleAllFilesFromDirectory(new Dictionary<string, string>
        {
            {"toc.yaml", _tocFileManager.GetDataObjectTocFile().RemoveEmptyLines()}
        }, tocFileFolderPath);
    }
    
    private void SaveCodesystemTocFile()
    {
        string? tocFileFolderPath = _directory.FindFolderPath("Terminologien");
        SimpleAllFilesFromDirectory(new Dictionary<string, string>
        {
            {"toc.yaml", _tocFileManager.GetCodeSystemTocFile().RemoveEmptyLines()}
        }, tocFileFolderPath);
    }
    
    private void SaveExtensionTocFile()
    {
        string? tocFileFolderPath = _directory.FindFolderPath("Extensions");
        SimpleAllFilesFromDirectory(new Dictionary<string, string>
        {
            {"toc.yaml", _tocFileManager.GetExtensionTocFile().RemoveEmptyLines()}
        }, tocFileFolderPath);
    }
    
    private void SaveCapabilityStatementTocFile()
    {
        string? tocFileFolderPath = _directory.FindFolderPath("CapabilityStatements");
        SimpleAllFilesFromDirectory(new Dictionary<string, string>
        {
            {"toc.yaml", _tocFileManager.GetCapabilitySatementTocFile().RemoveEmptyLines()}
        }, tocFileFolderPath);
    }

    private string? GetDataObjectPath(string subfolder)
    {
        string dataObjectFolderName = "Datenobjekte";
        string? fullPath = _directory.FindFolderPath(dataObjectFolderName) + "/" + subfolder;
        return fullPath;
    }

    private void SimpleAllFilesFromDirectory(IDictionary<string, string> files, string? fullPath)
    {
        if (!Directory.Exists($"{fullPath}"))
        {
            Directory.CreateDirectory($"{fullPath}");
        }

        foreach (KeyValuePair<string, string> f in files)
        {
            string file = $"{fullPath}/{f.Key}";
            File.Create(file).Dispose();
            File.WriteAllText(file, f.Value);
            _userInteractionHandler.Send($"{file} has been created");
        }
    }

    [GeneratedRegex("[^/]+$")]
    private static partial Regex LastPartOfCanonical();
}