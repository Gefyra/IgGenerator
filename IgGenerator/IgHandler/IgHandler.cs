using Hl7.Fhir.Model;
using IgGenerator.DataObjectHandling;
using IgGenerator.ResourceHandling;

namespace IgGenerator.IgHandler;

public class IgHandler : IIgHandler
{

    private readonly IResourceHandler _resourceHandler;
    private DataObjectTemplateHandler _templateHandler;
    private readonly IResourceFileHandler _resourceFileHandler;

    public IgHandler(IResourceHandler resourceHandler, DataObjectTemplateHandler templateHandler, IResourceFileHandler resourceFileHandler)
    {
        _resourceHandler = resourceHandler;
        _templateHandler = templateHandler;
        _resourceFileHandler = resourceFileHandler;
    }


    public IDictionary<string, IDictionary<string, string>> ApplyTemplateToAllSupportedProfiles()
    {
        _resourceFileHandler.StartWorkflow();
        IEnumerable<string>? supportedProfiles = _resourceHandler.ExtractSupportedProfiles();

        IDictionary<string, IDictionary<string, string>> result = new Dictionary<string, IDictionary<string, string>>();

        foreach (string supportedProfile in supportedProfiles)
        {
            StructureDefinition profileSd = _resourceHandler.GetStructureDefinition(supportedProfile);
            IDataObjectVariables variables = ExtractVariablesFromStructureDefinition(profileSd);
            
            IDictionary<string, string> chapter = _templateHandler.ApplyVariables(variables);
            result.Add(supportedProfile, chapter);
        }

        return result;
    }

    private static IDataObjectVariables ExtractVariablesFromStructureDefinition(StructureDefinition profileSd)
    {
        IDataObjectVariables variables = CreateDataObjectVariables
            .WithResourceName(profileSd.Type)
            .WithCanonical(profileSd.UrlElement.Value)
            .WithCoreUrl(profileSd.BaseDefinition)
            .WithNoExample(); //TODO ExampleSelection
        return variables;
    }
}

public interface IIgHandler
{
    public IDictionary<string, IDictionary<string, string>> ApplyTemplateToAllSupportedProfiles();
}