using System.Text.RegularExpressions;
using IgGenerator.DataObjectHandling.Interfaces;
using M31.FluentApi.Attributes;

namespace IgGenerator.DataObjectHandling;

[FluentApi]
public partial class DataObjectVariables : IDataObjectVariables
{
    [FluentMember(0)]
    public string? ResourceName { get; set; }
    [FluentMember(1)]
    public string? Canonical { get; set; }
    [FluentMember(3)]
    [FluentNullable("WithNoBaseUrl")]
    public string? BaseUrl { get; set; }
    [FluentMember(4)]
    [FluentNullable("WithNoExample")]
    public IDictionary<string, string>? ExampleNamesAndIds { get; set; }

    public string GetFilename()
    {
        return LastPartOfCanonical().Match(Canonical).Value;
    }
    
    
    [GeneratedRegex("[^/]+$")]
    private static partial Regex LastPartOfCanonical();
}

[FluentApi]
public class DataObjectTerminologyVariables : IDataObjectTerminologyVariables
{
    [FluentMember(0)]
    public string? TerminologyName { get; set; }
    [FluentMember(1)]
    public string? Canonical { get; set; }
}