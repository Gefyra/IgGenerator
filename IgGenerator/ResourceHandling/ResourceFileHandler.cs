using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Source;
using IgGenerator.ConsoleHandling.Interfaces;
using IgGenerator.ResourceHandling.Interfaces;

namespace IgGenerator.ResourceHandling;

public class ResourceFileHandler : IResourceFileHandler
{
    private const string JSON_EXTENSION = "*.json";
    private const string CAPABILITY_STATEMENT_PATTERN = "CapabilityStatement";
    
    private readonly IUserInteractionHandler _userInteractionHandler;
    private readonly FhirJsonParser _parser;
    private string? _folderPath;
    private CachedResolver? _resolver;

    public CapabilityStatement? CapabilityStatement { get; private set; }
    public IEnumerable<FileInfo>? AllJsonFiles { get; private set; }

    public ResourceFileHandler(IUserInteractionHandler userInteractionHandler)
    {
        _userInteractionHandler = userInteractionHandler ?? throw new ArgumentNullException(nameof(userInteractionHandler));
        _parser = new FhirJsonParser();
    }

    public void StartConsoleWorkflow()
    {
        _folderPath = _userInteractionHandler.GetString("Path of Resources:");
        
        if (string.IsNullOrEmpty(_folderPath) || !Directory.Exists(_folderPath))
        {
            throw new DirectoryNotFoundException($"Invalid resource directory path: {_folderPath}");
        }

        LoadJsonFiles();
        FindCapabilityStatement();
    }

    private void LoadJsonFiles()
    {
        try
        {
            var directory = new DirectoryInfo(_folderPath!);
            AllJsonFiles = directory.EnumerateFiles(JSON_EXTENSION, SearchOption.AllDirectories).ToArray();
            
            if (!AllJsonFiles.Any())
            {
                Console.WriteLine($"Warning: No JSON files found in {_folderPath}");
            }
            else
            {
                Console.WriteLine($"Found {AllJsonFiles.Count()} JSON files in {_folderPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading JSON files: {ex.Message}");
            AllJsonFiles = Array.Empty<FileInfo>();
        }
    }

    private void FindCapabilityStatement()
    {
        if (AllJsonFiles == null) return;

        var capabilityStatementFiles = AllJsonFiles
            .Where(f => f.Name.Contains(CAPABILITY_STATEMENT_PATTERN))
            .ToArray();

        if (!capabilityStatementFiles.Any())
        {
            Console.WriteLine("No CapabilityStatement files found.");
            return;
        }

        var selectedFile = SelectCapabilityStatementFile(capabilityStatementFiles);
        ParseCapabilityStatement(selectedFile);
    }

    private FileInfo SelectCapabilityStatementFile(FileInfo[] files)
    {
        if (files.Length == 1) 
        {
            Console.WriteLine($"Using CapabilityStatement: {files[0].Name}");
            return files[0];
        }

        Console.WriteLine("Multiple CapabilityStatement files found:");
        for (int i = 0; i < files.Length; i++)
        {
            Console.WriteLine($"{i}: {files[i].Name}");
        }

        var selectedIndex = _userInteractionHandler.GetNumber(
            "Which one? (Type number, default 0):", 0);

        Console.WriteLine($"Selected CapabilityStatement: {files[selectedIndex].Name}");
        return files[selectedIndex];
    }

    private void ParseCapabilityStatement(FileInfo file)
    {
        try
        {
            CapabilityStatement = _parser.Parse<CapabilityStatement>(File.ReadAllText(file.FullName));
            Console.WriteLine($"Successfully loaded CapabilityStatement from {file.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing CapabilityStatement from {file.Name}: {ex.Message}");
            CapabilityStatement = null;
        }
    }

    public CachedResolver GetCachedResolver()
    {
        if (_resolver != null) return _resolver;
        
        if (string.IsNullOrEmpty(_folderPath))
        {
            throw new InvalidOperationException("Resource folder path not initialized. Call StartConsoleWorkflow first.");
        }

        _resolver = new CachedResolver(new MultiResolver(new DirectorySource(_folderPath)));
        Console.WriteLine("Initialized FHIR resource resolver");
        return _resolver;
    }
}