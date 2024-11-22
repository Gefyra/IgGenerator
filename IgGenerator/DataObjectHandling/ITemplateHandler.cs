namespace IgGenerator.DataObjectHandling;

public interface ITemplateHandler
{
    public IDictionary<string, string> ApplyProfileVariables(IDataObjectVariables variables);
    public KeyValuePair<string, string> ApplyTermVariables(IDataObjectTerminologyVariables variables);
    public KeyValuePair<string, string> ApplyExtensionVariables(IDataObjectVariables variables);
    public Dictionary<string, string> CopyPasteFiles { get; }
    public KeyValuePair<string, string> DataObjectTocTemplate { get; }
    public KeyValuePair<string, string> CodeSystemTocTemplate { get; }
    public KeyValuePair<string, string> ExtensionTocTemplate { get; }
    public string ApplyTocList(string content, IEnumerable<IVariable> variables);
}