namespace IgGenerator.DataObjectHandling;

public interface IDataObjectTemplateHandler
{
    public IDictionary<string, string> ApplyVariables(IDataObjectVariables variables);
}