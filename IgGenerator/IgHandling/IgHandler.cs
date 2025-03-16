using Hl7.Fhir.Model;
using IgGenerator.DataObjectHandling.Interfaces;
using IgGenerator.IgHandling.Interfaces;
using IgGenerator.ResourceHandling.Interfaces;

namespace IgGenerator.IgHandling;

public class IgHandler : IIgHandler
{
    private readonly IResourceHandler _resourceHandler;
    private readonly ITemplateHandler _templateHandler;
    private readonly ITocFileManager _tocFileManager;

    public IgHandler(
        IResourceHandler resourceHandler,
        ITemplateHandler templateHandler,
        ITocFileManager tocFileManager)
    {
        _resourceHandler = resourceHandler ?? throw new ArgumentNullException(nameof(resourceHandler));
        _templateHandler = templateHandler ?? throw new ArgumentNullException(nameof(templateHandler));
        _tocFileManager = tocFileManager ?? throw new ArgumentNullException(nameof(tocFileManager));
    }

    public IDictionary<string, IDictionary<string, string>> ApplyTemplateToAllSupportedProfiles()
    {
        IEnumerable<string> supportedProfiles = _resourceHandler.ExtractSupportedProfiles();
        IDictionary<string, IDictionary<string, string>> result = new Dictionary<string, IDictionary<string, string>>();

        Console.WriteLine("Processing supported profiles...");
        foreach (string supportedProfile in supportedProfiles)
        {
            try
            {
                ProcessSupportedProfile(supportedProfile, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing profile {supportedProfile}: {ex.Message}");
            }
        }

        Console.WriteLine($"Processed {result.Count} profiles successfully");
        return result;
    }

    private void ProcessSupportedProfile(string supportedProfile, IDictionary<string, IDictionary<string, string>> result)
    {
        if (result.ContainsKey(supportedProfile))
        {
            Console.WriteLine($"Profile {supportedProfile} already processed, skipping...");
            return;
        }

        StructureDefinition? profileSd = _resourceHandler.GetStructureDefinition(supportedProfile);
        if (profileSd == null)
        {
            Console.WriteLine($"No StructureDefinition found for profile {supportedProfile}, skipping...");
            return;
        }

        Console.WriteLine($"Processing profile: {supportedProfile}");
        StructureDefinitionVariables variables = CreateStructureDefinitionVariables(profileSd, supportedProfile);
        IDictionary<string, string> chapter = _templateHandler.ApplyDataObjectVariables(variables);
        
        result.Add(supportedProfile, chapter);
        _tocFileManager.RegisterVariable(variables);
        Console.WriteLine($"Successfully processed profile: {supportedProfile}");
    }

    private StructureDefinitionVariables CreateStructureDefinitionVariables(
        StructureDefinition profileSd, 
        string supportedProfile)
    {
        StructureDefinitionVariables variables = new(profileSd)
        {
            Examples = _resourceHandler.GetExamplesForProfile(supportedProfile)
        };
        return variables;
    }

    public IDictionary<string, string> ApplyTemplateToCodeSystems()
    {
        IEnumerable<CodeSystem> codeSystems = _resourceHandler.GetCodeSystems();
        IDictionary<string, string> result = new Dictionary<string, string>();

        Console.WriteLine("Processing CodeSystems...");
        foreach (CodeSystem codeSystem in codeSystems)
        {
            try
            {
                ProcessCodeSystem(codeSystem, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing CodeSystem {codeSystem.Name}: {ex.Message}");
            }
        }

        Console.WriteLine($"Processed {result.Count} CodeSystems successfully");
        return result;
    }

    private void ProcessCodeSystem(CodeSystem codeSystem, IDictionary<string, string> result)
    {
        Console.WriteLine($"Processing CodeSystem: {codeSystem.Name}");
        CodeSystemVariables variables = new(codeSystem);
        result.Add(_templateHandler.ApplyVariables(variables));
        _tocFileManager.RegisterVariable(variables);
        Console.WriteLine($"Successfully processed CodeSystem: {codeSystem.Name}");
    }
    
    public IDictionary<string, string> ApplyTemplateToCapabilityStatement()
    {
        IDictionary<string, string> result = new Dictionary<string, string>();

        // Verarbeite alle Capability Statements
        foreach (var capabilityStatement in _resourceHandler.GetCapabilityStatements())
        {
            if (capabilityStatement == null)
            {
                Console.WriteLine("No capability statement found.");
                continue;
            }

            try
            {
                Console.WriteLine($"Processing CapabilityStatement: {capabilityStatement.Name ?? capabilityStatement.Id ?? "unnamed"}");
                ProcessCapabilityStatement(capabilityStatement, result);
                Console.WriteLine($"Successfully processed CapabilityStatement: {capabilityStatement.Name ?? capabilityStatement.Id ?? "unnamed"}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing CapabilityStatement: {ex.Message}");
            }
        }

        return result;
    }

    private void ProcessCapabilityStatement(CapabilityStatement capabilityStatement, IDictionary<string, string> result)
    {
        CapabilityStatementVariables variables = new(capabilityStatement);
        
        result.Add(_templateHandler.ApplyVariables(variables));
        _tocFileManager.RegisterVariable(variables);
    }

    public IDictionary<string, string> ApplyTemplateToExtensions()
    {
        IEnumerable<(string name, string canonical)> extensions = _resourceHandler.GetUsedExtensions();
        IDictionary<string, string> result = new Dictionary<string, string>();

        Console.WriteLine("Processing Extensions...");
        foreach ((string name, string canonical) extension in extensions)
        {
            try
            {
                ProcessExtension(extension, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing Extension {extension.name}: {ex.Message}");
            }
        }

        Console.WriteLine($"Processed {result.Count} Extensions successfully");
        return result;
    }

    private void ProcessExtension((string name, string canonical) extension, IDictionary<string, string> result)
    {
        Console.WriteLine($"Processing Extension: {extension.name}");
        ExtensionVariables variables = new(extension.name, extension.canonical);
        result.Add(_templateHandler.ApplyVariables(variables));
        _tocFileManager.RegisterVariable(variables);
        Console.WriteLine($"Successfully processed Extension: {extension.name}");
    }
}