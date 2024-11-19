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
            Directory.CreateDirectory(folder);
            foreach (KeyValuePair<string, string> dataObjectFile in dataObject.Value)
            {
                string file = $"{folder}/{_namingManipulationHandler.FilterPartFromFilename(dataObjectFile.Key)}";
                File.Create(file).Dispose();
                File.WriteAllText(file, dataObjectFile.Value);
                _userInteractionHandler.Send($"{file} has been created");
            }
        }
    }

    public void SaveExtractedCodeSystemFiles(IDictionary<string, string> extractedCodeSystems)
    {
        string dataObjectFolderName = "Datenobjekte";
        string? fullPath = _directory.FindFolderPath(dataObjectFolderName);
        const string terminologyFolderName = "Terminologien";

        if (!Directory.Exists($"{fullPath}/{terminologyFolderName}"))
        {
            Directory.CreateDirectory($"{fullPath}/{terminologyFolderName}");
        }
        
        foreach (KeyValuePair<string, string> codeSystem in extractedCodeSystems)
        {
            string file = $"{fullPath}/{terminologyFolderName}/{codeSystem.Key}";
            File.Create(file).Dispose();
            File.WriteAllText(file, codeSystem.Value);
            _userInteractionHandler.Send($"{file} has been created");
        }
    }

    [GeneratedRegex("[^/]+$")]
    private static partial Regex LastPartOfCanonical();
}