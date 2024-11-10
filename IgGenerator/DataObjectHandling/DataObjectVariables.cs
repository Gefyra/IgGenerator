using M31.FluentApi.Attributes;

namespace IgGenerator.DataObjectHandling;

[FluentApi]
public class DataObjectVariables : IDataObjectVariables
{
    [FluentMember(0)]
    public string? ResourceName { get; set; }
    [FluentMember(1)]
    public string? Canonical { get; set; }
    [FluentMember(3)]
    public string? CoreUrl { get; set; }
    [FluentMember(4)]
    [FluentNullable("WithNoExample")]
    public IDictionary<string, string>? ExampleNamesAndIds { get; set; }
}