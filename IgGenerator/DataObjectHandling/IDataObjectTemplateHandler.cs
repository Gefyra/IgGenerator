namespace IgGenerator.DataObjectHandling;

public interface IDataObjectTemplateHandler
{
    public IDictionary<string, string> ApplyProfileVariables(IDataObjectVariables variables);
    public KeyValuePair<string, string> ApplyTermVariables(IDataObjectTerminologyVariables variables);
    public KeyValuePair<string, string> ApplyExtensionVariables(IDataObjectVariables variables);
    public Dictionary<string, string> TopLevelDataObjectFixFiles { get; }
}