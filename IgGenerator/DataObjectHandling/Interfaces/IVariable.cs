using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling.Interfaces;

public interface IVariable
{
    public const string VARNAME_SD_NAME = "$$StructureDefinition.name";
    public const string VARNAME_SD_URL = "$$StructureDefinition.url";
    public const string VARNAME_SD_TYPE = "$$StructureDefinition.type";
    public const string VARNAME_SD_BASEURL = "$$StructureDefinition.baseUrl";
    public const string VARNAME_EXAMPLENAME = "%igg.exampleName";
    public const string VARNAME_EXAMPLEID = "%igg.exampleId";
    public const string VARNAME_FILENAME = "$$Filename";
    public const string VARNAME_FILENAME_UMLAUT = "$$FilenameUmlaut";
    public const string VARNAME_CS_NAME = "$$CodeSystem.name";
    public const string VARNAME_CS_URL = "$$CodeSystem.url";
    public const string VARNAME_CAPSTMT_NAME = "$$CapabilityStatement.name";
    public const string VARNAME_CAPSTMT_URL = "$$CapabilityStatement.url";

    public const string STARTEXAMPLE = "%igg.startExample";
    public const string ENDEXAMPLE = "%igg.endExample";

    public const string STARTTOCOBJECT = "$$startTocObject";
    public const string ENDTOCOBJECT = "$$endTocObject";

    public string ApplyVariables(string content);
    public void ApplyNamingManipulation(INamingManipulationHandler handler);
}