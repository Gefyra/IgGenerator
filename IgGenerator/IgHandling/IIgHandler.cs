namespace IgGenerator.IgHandling;

public interface IIgHandler
{
    public IDictionary<string, IDictionary<string, string>> ApplyTemplateToAllSupportedProfiles();
    public IDictionary<string, string> ApplyTemplateToCodeSystems();
}