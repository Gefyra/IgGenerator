namespace IgGenerator.DataObjectHandling;

public interface IDataObjectTemplateHandler
{
    public IDictionary<string, string> ApplyVariables(IDataObjectVariables variables);
    public KeyValuePair<string, string> ApplyVariables(IDataObjectTerminologyVariables variables);
    public Dictionary<string, string> TopLevelDataObjectFixFiles { get; }
}