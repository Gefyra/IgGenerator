using System.Text.RegularExpressions;
using IgGenerator.ConsoleHandling;
using IgGenerator.Helpers;

namespace IgGenerator.IgHandling;

public partial class IgFileHandler :IIgFileHandler
{
    private readonly IUserInteractionHandler _userInteractionHandler;
    private readonly INamingManipulationHandler _namingManipulationHandler;
    private DirectoryInfo _directory;
    public string igFolderPath { get; private set; }

    public IgFileHandler(IUserInteractionHandler userInteractionHandler, INamingManipulationHandler namingManipulationHandler)
    {
        _userInteractionHandler = userInteractionHandler;
        _namingManipulationHandler = namingManipulationHandler;
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
            //TODO Besseres Errorhandling
            _userInteractionHandler.SendAndExit($"There is no subfolder {dataObjectFolderName} at {_directory.FullName}");
        }
        
        foreach (KeyValuePair<string, IDictionary<string, string>> dataObject in extractedDataObjects)
        {
            string match = _namingManipulationHandler.FilterPartFromFilename(LastPartOfCanonical().Match(dataObject.Key).Value);
            string folder = $"{fullPath}/Datenobjekt_{match}";
            SimpleAllFilesFromDirectory(dataObject.Value, folder);
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

    private string? GetDataObjectPath(string subfolder)
    {
        string dataObjectFolderName = "Datenobjekte";
        string? fullPath = _directory.FindFolderPath(dataObjectFolderName) + "/" + subfolder;
        return fullPath;
    }

    private void SimpleAllFilesFromDirectory(IDictionary<string, string> files, string? fullPath )
    {
        if (!Directory.Exists($"{fullPath}"))
        {
            Directory.CreateDirectory($"{fullPath}");
        }

        foreach (KeyValuePair<string, string> extension in files)
        {
            string file = $"{fullPath}/{extension.Key}";
            File.Create(file).Dispose();
            File.WriteAllText(file, extension.Value);
            _userInteractionHandler.Send($"{file} has been created");
        }
    }

    [GeneratedRegex("[^/]+$")]
    private static partial Regex LastPartOfCanonical();
}