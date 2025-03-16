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

    public List<CapabilityStatement> CapabilityStatements { get; private set; } = new();
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
        FindCapabilityStatements();
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

    private void FindCapabilityStatements()
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

        var selectedFiles = SelectCapabilityStatementFiles(capabilityStatementFiles);
        
        CapabilityStatements.Clear();
        foreach (var file in selectedFiles)
        {
            var capabilityStatement = ParseCapabilityStatement(file);
            if (capabilityStatement != null)
            {
                CapabilityStatements.Add(capabilityStatement);
            }
        }
        
        Console.WriteLine($"Loaded {CapabilityStatements.Count} capability statements.");
    }

    private IEnumerable<FileInfo> SelectCapabilityStatementFiles(FileInfo[] files)
    {
        if (files.Length == 1) 
        {
            Console.WriteLine($"Using CapabilityStatement: {files[0].Name}");
            return new[] { files[0] };
        }

        Console.WriteLine("Multiple CapabilityStatement files found:");
        for (int i = 0; i < files.Length; i++)
        {
            Console.WriteLine($"{i}: {files[i].Name}");
        }

        var input = _userInteractionHandler.GetString(
            "Which ones? (Type numbers separated by commas, or 'all' for all, default 0):");
        
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine($"Selected CapabilityStatement: {files[0].Name}");
            return new[] { files[0] };
        }
        
        if (input.Trim().Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Selected all CapabilityStatements");
            return files;
        }
        
        var selectedIndices = input.Split(',')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => int.TryParse(s, out var index) ? index : -1)
            .Where(index => index >= 0 && index < files.Length)
            .Distinct()
            .ToArray();
        
        if (!selectedIndices.Any())
        {
            Console.WriteLine($"No valid indices provided. Using default: {files[0].Name}");
            return new[] { files[0] };
        }
        
        var selectedFiles = selectedIndices.Select(i => files[i]).ToArray();
        Console.WriteLine($"Selected {selectedFiles.Length} CapabilityStatements: {string.Join(", ", selectedFiles.Select(f => f.Name))}");
        return selectedFiles;
    }

    private CapabilityStatement? ParseCapabilityStatement(FileInfo file)
    {
        try
        {
            var capabilityStatement = _parser.Parse<CapabilityStatement>(File.ReadAllText(file.FullName));
            Console.WriteLine($"Successfully loaded CapabilityStatement from {file.Name}");
            return capabilityStatement;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing CapabilityStatement from {file.Name}: {ex.Message}");
            return null;
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