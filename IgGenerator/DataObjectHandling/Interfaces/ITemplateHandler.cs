namespace IgGenerator.DataObjectHandling.Interfaces;

public interface ITemplateHandler
{
    public IDictionary<string, string> ApplyProfileVariables(IVariable variables);
    public KeyValuePair<string, string> ApplyVariables(IVariable variables);
    public Dictionary<string, string> CopyPasteFiles { get; }
    public KeyValuePair<string, string> DataObjectTocTemplate { get; }
    public KeyValuePair<string, string> CodeSystemTocTemplate { get; }
    public KeyValuePair<string, string> ExtensionTocTemplate { get; }
    public string ApplyTocList(string content, IEnumerable<IVariable> variables);
}