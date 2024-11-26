namespace IgGenerator.DataObjectHandling.Interfaces;

public interface IDataObjectVariables : IVariable
{
    public string ResourceName { get; set; }
    public string Canonical { get; set; }
    public string? BaseUrl { get; set; }
    public IDictionary<string, string>? ExampleNamesAndIds { get; set; }
    public string GetFilename();
}