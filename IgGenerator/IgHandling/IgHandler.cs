using Hl7.Fhir.Model;
using IgGenerator.DataObjectHandling;
using IgGenerator.DataObjectHandling.Interfaces;
using IgGenerator.IgHandling.Interfaces;
using IgGenerator.ResourceHandling;
using IgGenerator.ResourceHandling.Interfaces;

namespace IgGenerator.IgHandling;

public class IgHandler(
    IResourceHandler resourceHandler,
    ITemplateHandler templateHandler,
    IResourceFileHandler resourceFileHandler,
    IIgFileHandler igFileHandler,
    ITocFileManager tocFileManager)
    : IIgHandler
{

    public IDictionary<string, IDictionary<string, string>> ApplyTemplateToAllSupportedProfiles()
    {
        IEnumerable<string>? supportedProfiles = resourceHandler.ExtractSupportedProfiles();

        IDictionary<string, IDictionary<string, string>> result = new Dictionary<string, IDictionary<string, string>>();

        foreach (string supportedProfile in supportedProfiles)
        {
            StructureDefinition? profileSd = resourceHandler.GetStructureDefinition(supportedProfile);
            if (profileSd == null)
            {
                continue;
            }
            IDataObjectVariables variables = ExtractVariablesFromStructureDefinition(profileSd);
            
            IDictionary<string, string> chapter = templateHandler.ApplyProfileVariables(variables);
            result.Add(supportedProfile, chapter);

            tocFileManager?.RegisterDataObject(variables);
        }
        return result;
    }

    public IDictionary<string, string> ApplyTemplateToCodeSystems()
    {
        IEnumerable<CodeSystem> codeSystems = resourceHandler.GetCodeSystems();

        IDictionary<string, string> result = new Dictionary<string, string>();

        foreach (CodeSystem codeSystem in codeSystems)
        {
            IDataObjectTerminologyVariables variables = ExtractVariablesFromCodeSystem(codeSystem);
            result.Add(templateHandler.ApplyTermVariables(variables));
            
            tocFileManager?.RegisterCodesystem(variables);
        }

        return result;
    }

    public IDictionary<string, string> ApplyTemplateToExtensions()
    {
        IEnumerable<(string name, string canonical)> extensions = resourceHandler.GetUsedExtensions();

        IDictionary<string, string> result = new Dictionary<string, string>();

        foreach ((string name, string canonical) extension in extensions)
        {
            IDataObjectVariables variables = ExtractVariablesFromExtensionTupel(extension);
            result.Add(templateHandler.ApplyExtensionVariables(variables));
            
            tocFileManager?.RegisterExtension(variables);
        }

        return result;
    }

    private static IDataObjectVariables ExtractVariablesFromStructureDefinition(StructureDefinition profileSd)
    {
        IDataObjectVariables variables = CreateDataObjectVariables
            .WithResourceName(profileSd.Type)
            .WithCanonical(profileSd.UrlElement.Value)
            .WithBaseUrl(profileSd.BaseDefinition)
            .WithNoExample(); //TODO ExampleSelection
        return variables;
    }
    
    private static IDataObjectTerminologyVariables ExtractVariablesFromCodeSystem(CodeSystem codeSystem) =>
        CreateDataObjectTerminologyVariables
            .WithTerminologyName(codeSystem.Name)
            .WithCanonical(codeSystem.UrlElement.Value);
    
    private static IDataObjectVariables ExtractVariablesFromExtensionTupel((string name, string canonical) extension) =>
        CreateDataObjectVariables
            .WithResourceName(extension.name)
            .WithCanonical(extension.canonical)
            .WithNoBaseUrl().WithNoExample();
}