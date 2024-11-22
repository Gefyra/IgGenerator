namespace IgGenerator.IgHandling.Interfaces;

public interface IIgHandler
{
    public IDictionary<string, IDictionary<string, string>> ApplyTemplateToAllSupportedProfiles();
    public IDictionary<string, string> ApplyTemplateToCodeSystems();
    public IDictionary<string, string> ApplyTemplateToExtensions();
}