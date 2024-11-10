namespace IG_Generator;

public interface IIgDataObjectVariables
{
    public const string VARNAME_RESOURCENAME = "%igg.resourceName";
    public const string VARNAME_CANONICAL = "%igg.canonical";
    public const string VARNAME_COREURL = "%igg.coreUrl";
    public const string VARNAME_EXAMPLENAME = "%igg.exampleName";
    public const string VARNAME_EXAMPLEID = "%igg.exampleId";
    
    public const string STARTEXAMPLE = "%igg.startExample";
    public const string ENDEXAMPLE = "%igg.endExample";

    public string? ResourceName { get; set; }
    public string? Canonical { get; set; }
    public string? CoreUrl { get; set; }
    public IDictionary<string, string>? ExampleNamesAndIds { get; set; }
}