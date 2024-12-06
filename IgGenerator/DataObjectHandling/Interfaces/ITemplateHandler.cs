namespace IgGenerator.DataObjectHandling.Interfaces;

public interface ITemplateHandler
{
    public IDictionary<string, string> ApplyProfileVariables(IVariable variables);
    public KeyValuePair<string, string> ApplyVariables(IVariable variables);
    public string ApplyTocList(string content, IEnumerable<IVariable> variables);
    public IEnumerable<Template> GetTemplate(TemplateType[] templateTypes);
}