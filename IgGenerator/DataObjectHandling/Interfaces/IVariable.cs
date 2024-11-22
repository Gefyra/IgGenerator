namespace IgGenerator.DataObjectHandling.Interfaces;

public interface IVariable
{
    public const string VARNAME_RESOURCENAME = "%igg.resourceName";
    public const string VARNAME_CANONICAL = "%igg.canonical";
    public const string VARNAME_COREURL = "%igg.coreUrl";
    public const string VARNAME_EXAMPLENAME = "%igg.exampleName";
    public const string VARNAME_EXAMPLEID = "%igg.exampleId";
    public const string VARNAME_FILENAME = "%igg.filename";
    public const string VARNAME_FILENAME_UMLAUT = "%igg.filenameUmlaut";
    
    public const string STARTEXAMPLE = "%igg.startExample";
    public const string ENDEXAMPLE = "%igg.endExample";
    
    public const string STARTTOCOBJECT = "%igg.startTocObject";
    public const string ENDTOCOBJECT = "%igg.endTocObject";
    public const string VARNAME_TERMNAME = "%igg.termname";
}