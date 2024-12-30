using Hl7.Fhir.Model;
using IgGenerator.DataObjectHandling.Interfaces;
using IgGenerator.IgHandling.Interfaces;
using IgGenerator.ResourceHandling.Interfaces;

namespace IgGenerator.IgHandling;

public class IgHandler(
    IResourceHandler resourceHandler,
    ITemplateHandler templateHandler,
    ITocFileManager tocFileManager)
    : IIgHandler
{

    public IDictionary<string, IDictionary<string, string>> ApplyTemplateToAllSupportedProfiles()
    {
        IEnumerable<string>? supportedProfiles = resourceHandler.ExtractSupportedProfiles();

        IDictionary<string, IDictionary<string, string>> result = new Dictionary<string, IDictionary<string, string>>();

        foreach (string supportedProfile in supportedProfiles)
        {
            if (result.ContainsKey(supportedProfile))
            {
                continue;
            }
            StructureDefinition? profileSd = resourceHandler.GetStructureDefinition(supportedProfile);
            if (profileSd == null)
            {
                continue;
            }
            StructureDefinitionVariables variables = new(profileSd);
            variables.Examples = resourceHandler.GetExamplesForProfile(supportedProfile);
            
            IDictionary<string, string> chapter = templateHandler.ApplyDataObjectVariables(variables);
            result.Add(supportedProfile, chapter);

            tocFileManager?.RegisterVariable(variables);
        }
        return result;
    }

    public IDictionary<string, string> ApplyTemplateToCodeSystems()
    {
        IEnumerable<CodeSystem> codeSystems = resourceHandler.GetCodeSystems();

        IDictionary<string, string> result = new Dictionary<string, string>();

        foreach (CodeSystem codeSystem in codeSystems)
        {
            CodeSystemVariables variables = new(codeSystem);
            result.Add(templateHandler.ApplyVariables(variables));
            
            tocFileManager?.RegisterVariable(variables);
        }

        return result;
    }
    
    public IDictionary<string, string> ApplyTemplateToCapabilityStatement()
    {
        CapabilityStatement? capabilityStatement = resourceHandler.GetCapabilityStatement();

        IDictionary<string, string> result = new Dictionary<string, string>();

        CapabilityStatementVariables variables = new(capabilityStatement);
        result.Add(templateHandler.ApplyVariables(variables));
        
        tocFileManager?.RegisterVariable(variables);

        return result;
    }

    public IDictionary<string, string> ApplyTemplateToExtensions()
    {
        IEnumerable<(string name, string canonical)> extensions = resourceHandler.GetUsedExtensions();

        IDictionary<string, string> result = new Dictionary<string, string>();

        foreach ((string name, string canonical) extension in extensions)
        {
            ExtensionVariables variables = new(extension.name, extension.canonical);
            result.Add(templateHandler.ApplyVariables(variables));
            
            tocFileManager?.RegisterVariable(variables);
        }

        return result;
    }
}