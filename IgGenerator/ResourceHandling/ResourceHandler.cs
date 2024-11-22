using System.Text.RegularExpressions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using IgGenerator.ConsoleHandling;

namespace IgGenerator.ResourceHandling;

public partial class ResourceHandler : IResourceHandler
{
    private readonly IResourceFileHandler _fileHandler;
    private readonly IUserInteractionHandler _userInteractionHandler;
    private readonly FhirJsonParser _parser = new();

    public ResourceHandler(IResourceFileHandler fileHandler, IUserInteractionHandler userInteractionHandler)
    {
        _fileHandler = fileHandler;
        _userInteractionHandler = userInteractionHandler;
    }

    public IEnumerable<string>? ExtractSupportedProfiles()
    {
        return _fileHandler.CapabilityStatement?.Rest
            .Where(e => e.Mode == CapabilityStatement.RestfulCapabilityMode.Server)
            .SelectMany(e => e.Resource.SelectMany(r => r.SupportedProfile));
    }

    public IEnumerable<CodeSystem> GetCodeSystems()
    {
        IEnumerable<FileInfo>? sdFile = _fileHandler.AllJsonFiles?.Where(e => e.Name.StartsWith($"CodeSystem-")).ToArray();
        return sdFile?.Select(e=>_parser.Parse<CodeSystem>(File.ReadAllText(e.FullName))) ?? Array.Empty<CodeSystem>();
    }

    public IEnumerable<(string name, string canonical)> GetUsedExtensions()
    {
        IEnumerable<FileInfo>? sdFile = _fileHandler.AllJsonFiles?.Where(e => e.Name.StartsWith($"StructureDefinition-")).ToArray();

        List<(string name, string canonical)> usedExtensions = new();
        
        foreach (FileInfo fileInfo in sdFile!)
        {
            string sdContent = File.ReadAllText(fileInfo.FullName);
            StructureDefinition sd = _parser.Parse<StructureDefinition>(sdContent);

            foreach (ElementDefinition elementDefinition in sd.Differential.Element.Where(e=>e.Type.Any(t=>t.Code == "Extension")))
            {
                (string name, string canonical) tuple = ExtractExtensionTuple(sd.Type, elementDefinition);
                if (usedExtensions.All(e => e.canonical != tuple.canonical))
                {
                    usedExtensions.Add(tuple);
                }
            }
        }

        return usedExtensions.AsEnumerable();
    }

    private static (string name, string canonical) ExtractExtensionTuple(string resourceName, ElementDefinition elementDefinition)
    {
        string name = $"{resourceName}-{elementDefinition.SliceName}"; //TODO Name bei Simplifier EP abfragen
        string? canonical = elementDefinition.Type.First(e => e.Code == "Extension").Profile.FirstOrDefault();
        return (name, canonical)!;
    }

    public StructureDefinition GetStructureDefinition(string supportedProfile)
    {
        const string pattern = @"[^/]+$";
        string match = LastPartOfCanonical().Match(supportedProfile).Value;
        IEnumerable<FileInfo>? sdFile = _fileHandler.AllJsonFiles?.Where(e => e.Name.Equals($"StructureDefinition-{match}.json")).ToArray();
        int number = 0;
        if (sdFile.Count() > 1)
        {
            for (int iii = 0; iii < sdFile.Count(); iii++)
            {
                Console.WriteLine($"{iii}: {sdFile.ElementAt(iii).Name}");
            }

            number = _userInteractionHandler.GetNumber("Which one? (Type number, default 0):", 0);
        }
        
        return _parser.Parse<StructureDefinition>(File.ReadAllText(sdFile.ElementAt(number).FullName));
    }

    [GeneratedRegex("[^/]+$")]
    private static partial Regex LastPartOfCanonical();
}