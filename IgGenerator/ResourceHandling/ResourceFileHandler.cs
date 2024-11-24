using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Source;
using IgGenerator.ConsoleHandling;
using IgGenerator.ConsoleHandling.Interfaces;
using IgGenerator.ResourceHandling.Interfaces;

namespace IgGenerator.ResourceHandling;

public class ResourceFileHandler(IUserInteractionHandler userInteractionHandler) : IResourceFileHandler
{
    private string _folderPath = null!;
    private readonly FhirJsonParser _parser = new();
    public CapabilityStatement? CapabilityStatement { get; private set; }
    public IEnumerable<FileInfo>? AllJsonFiles { get; private set; }
    private CachedResolver? _resolver;

    public void StartConsoleWorkflow()
    {
        _folderPath = userInteractionHandler.GetString("Path of Resources:");
        FindCapabilityStatement();
    }

    private void FindCapabilityStatement()
    {
        DirectoryInfo di = new(_folderPath);
        AllJsonFiles = di.EnumerateFiles("*.json", SearchOption.AllDirectories);

        IEnumerable<FileInfo> csFiles = AllJsonFiles.Where(f => f.Name.Contains("CapabilityStatement")).ToArray();
        int number = 0;
        if (csFiles.Count() > 1)
        {
            for (int iii = 0; iii < csFiles.Count(); iii++)
            {
                Console.WriteLine($"{iii}: {csFiles.ElementAt(iii).Name}");
            }

            number = userInteractionHandler.GetNumber("Which one? (Type number, default 0):", 0);
        }

        CapabilityStatement = _parser.Parse<CapabilityStatement>(File.ReadAllText(csFiles.ElementAt(number).FullName));
    }

    public CachedResolver GetCachedResolver() =>
        _resolver ??= new CachedResolver(
            new MultiResolver(
                new DirectorySource(_folderPath)));
}