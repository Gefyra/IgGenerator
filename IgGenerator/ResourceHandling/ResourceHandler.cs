using System.Text.RegularExpressions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using IgGenerator.ConsoleHandling.Interfaces;
using IgGenerator.ResourceHandling.Interfaces;

namespace IgGenerator.ResourceHandling;

public partial class ResourceHandler : IResourceHandler
{
    private const string STRUCTURE_DEFINITION_PREFIX = "StructureDefinition-";
    private const string CODE_SYSTEM_PREFIX = "CodeSystem-";
    
    private readonly IResourceFileHandler _fileHandler;
    private readonly IUserInteractionHandler _userInteractionHandler;
    private readonly FhirJsonParser _parser;
    private IEnumerable<Resource>? _resourcesCache;

    public ResourceHandler(
        IResourceFileHandler fileHandler, 
        IUserInteractionHandler userInteractionHandler)
    {
        _fileHandler = fileHandler ?? throw new ArgumentNullException(nameof(fileHandler));
        _userInteractionHandler = userInteractionHandler ?? throw new ArgumentNullException(nameof(userInteractionHandler));
        _parser = new FhirJsonParser();
    }

    public IEnumerable<string> ExtractSupportedProfiles()
    {
        if (_fileHandler.CapabilityStatement != null)
        {
            return ExtractProfilesFromCapabilityStatement();
        }
        
        return ExtractProfilesFromStructureDefinitions();
    }

    private IEnumerable<string> ExtractProfilesFromCapabilityStatement()
    {
        return _fileHandler.CapabilityStatement?.Rest
            .Where(e => e.Mode == CapabilityStatement.RestfulCapabilityMode.Server)
            .SelectMany(e => e.Resource.SelectMany(r => r.SupportedProfile))
            ?? Enumerable.Empty<string>();
    }

    private IEnumerable<string> ExtractProfilesFromStructureDefinitions()
    {
        var structureDefinitions = GetStructureDefinitionFiles()
            ?.Select(file => ParseResource<StructureDefinition>(file))
            .Where(sd => sd?.Type != "Extension");

        return structureDefinitions?
            .Where(sd => sd != null)
            .Select(sd => sd!.Url)
            ?? Enumerable.Empty<string>();
    }

    public IEnumerable<CodeSystem> GetCodeSystems()
    {
        var codeSystemFiles = _fileHandler.AllJsonFiles?
            .Where(e => e.Name.StartsWith(CODE_SYSTEM_PREFIX))
            .ToArray();

        return codeSystemFiles?
            .Select(file => ParseResource<CodeSystem>(file))
            .Where(cs => cs != null)
            .Select(cs => cs!)
            ?? Enumerable.Empty<CodeSystem>();
    }

    public CapabilityStatement? GetCapabilityStatement() => _fileHandler.CapabilityStatement;

    public IEnumerable<(string name, string canonical)> GetUsedExtensions()
    {
        var structureDefinitions = GetStructureDefinitionFiles();
        if (structureDefinitions == null) return Enumerable.Empty<(string, string)>();

        var usedExtensions = new HashSet<(string name, string canonical)>();
        
        foreach (var file in structureDefinitions)
        {
            var sd = ParseResource<StructureDefinition>(file);
            if (sd?.Differential?.Element == null) continue;

            var extensionElements = sd.Differential.Element
                .Where(e => e.Type.Any(t => t.Code == "Extension"));

            foreach (var element in extensionElements)
            {
                var extensionInfo = ExtractExtensionInfo(sd.Type, element);
                if (!string.IsNullOrEmpty(extensionInfo.canonical))
                {
                    usedExtensions.Add(extensionInfo);
                }
            }
        }

        return usedExtensions;
    }

    private static (string name, string canonical) ExtractExtensionInfo(
        string resourceName, 
        ElementDefinition elementDefinition)
    {
        var name = $"{resourceName}-{elementDefinition.SliceName}"; //TODO Name bei Simplifier EP abfragen
        var canonical = elementDefinition.Type
            .First(e => e.Code == "Extension")
            .Profile
            .FirstOrDefault();

        return (name, canonical ?? string.Empty);
    }

    public IEnumerable<Resource> GetExamplesForProfile(string supportedProfile)
    {
        EnsureResourceCache();
        
        return _resourcesCache?
            .Where(r => HasMatchingProfile(r, supportedProfile))
            ?? Enumerable.Empty<Resource>();
    }

    private void EnsureResourceCache()
    {
        if (_resourcesCache != null) return;
        
        _resourcesCache = _fileHandler.AllJsonFiles?
            .Select(file => ParseResource<Resource>(file))
            .Where(r => r != null)
            .Select(r => r!)
            .ToArray() ?? Array.Empty<Resource>();

        _userInteractionHandler.Send(
            $"Found {_resourcesCache.Count(e => e.Meta?.Profile.Any() == true)} examples");
    }

    private static bool HasMatchingProfile(Resource resource, string profile)
    {
        return resource.Meta?.Profile.Contains(profile) == true;
    }
    
    public StructureDefinition? GetStructureDefinition(string supportedProfile)
    {
        var structureDefinition = TryResolveStructureDefinition(supportedProfile);
        if (structureDefinition != null) return structureDefinition;

        var matchingFiles = FindMatchingStructureDefinitionFiles(supportedProfile);
        if (!matchingFiles.Any())
        {
            _userInteractionHandler.Send($"No structure definition found for canonical {supportedProfile}.");
            return null;
        }

        var selectedFile = SelectStructureDefinitionFile(matchingFiles);
        return ParseResource<StructureDefinition>(selectedFile);
    }

    private StructureDefinition? TryResolveStructureDefinition(string supportedProfile)
    {
        return _fileHandler.GetCachedResolver()
            .ResolveByCanonicalUriAsync(supportedProfile)
            .Result as StructureDefinition;
    }

    private IEnumerable<FileInfo> FindMatchingStructureDefinitionFiles(string supportedProfile)
    {
        var match = LastPartOfCanonical().Match(supportedProfile).Value;
        return _fileHandler.AllJsonFiles?
            .Where(e => e.Name.Equals($"{STRUCTURE_DEFINITION_PREFIX}{match}.json"))
            ?? Enumerable.Empty<FileInfo>();
    }

    private FileInfo SelectStructureDefinitionFile(IEnumerable<FileInfo> files)
    {
        if (files.Count() <= 1) return files.First();

        for (int i = 0; i < files.Count(); i++)
        {
            _userInteractionHandler.Send($"{i}: {files.ElementAt(i).Name}");
        }

        var selectedIndex = _userInteractionHandler.GetNumber(
            "Which one? (Type number, default 0):", 0);
        
        return files.ElementAt(selectedIndex);
    }

    private FileInfo[]? GetStructureDefinitionFiles()
    {
        return _fileHandler.AllJsonFiles?
            .Where(e => e.Name.StartsWith(STRUCTURE_DEFINITION_PREFIX))
            .ToArray();
    }

    private T? ParseResource<T>(FileInfo file) where T : Resource
    {
        try
        {
            return _parser.Parse<T>(File.ReadAllText(file.FullName));
        }
        catch (Exception ex)
        {
            _userInteractionHandler.Send($"Error parsing {file.Name}: {ex.Message}");
            return null;
        }
    }

    [GeneratedRegex("[^/]+$")]
    private static partial Regex LastPartOfCanonical();
}