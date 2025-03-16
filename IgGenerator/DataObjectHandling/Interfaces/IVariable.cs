using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling.Interfaces;

/// <summary>
/// Defines the contract for template variables that can be applied to templates.
/// This interface provides constants for variable placeholders and methods for variable manipulation.
/// </summary>
public interface IVariable
{
    #region Structure Definition Variables
    /// <summary>Structure Definition name placeholder</summary>
    public const string VARNAME_SD_NAME = "$$StructureDefinition.name";
    /// <summary>Structure Definition URL placeholder</summary>
    public const string VARNAME_SD_URL = "$$StructureDefinition.url";
    /// <summary>Structure Definition type placeholder</summary>
    public const string VARNAME_SD_TYPE = "$$StructureDefinition.type";
    /// <summary>Structure Definition base URL placeholder</summary>
    public const string VARNAME_SD_BASEURL = "$$StructureDefinition.baseUrl";
    #endregion

    #region Resource Variables
    /// <summary>Resource ID placeholder</summary>
    public const string VARNAME_R_ID = "$$Resource.id";
    #endregion

    #region Filename Variables
    /// <summary>Filename placeholder</summary>
    public const string VARNAME_FILENAME = "$$Filename";
    /// <summary>Filename with umlaut placeholder</summary>
    public const string VARNAME_FILENAME_UMLAUT = "$$FilenameUmlaut";
    #endregion

    #region CodeSystem Variables
    /// <summary>CodeSystem name placeholder</summary>
    public const string VARNAME_CS_NAME = "$$CodeSystem.name";
    /// <summary>CodeSystem URL placeholder</summary>
    public const string VARNAME_CS_URL = "$$CodeSystem.url";
    #endregion

    #region CapabilityStatement Variables
    /// <summary>CapabilityStatement name placeholder</summary>
    public const string VARNAME_CAPSTMT_NAME = "$$CapabilityStatement.name";
    /// <summary>CapabilityStatement URL placeholder</summary>
    public const string VARNAME_CAPSTMT_URL = "$$CapabilityStatement.url";
    #endregion

    #region Template Section Markers
    /// <summary>Start marker for TOC object section</summary>
    public const string STARTTOCOBJECT = "$$startTocObject";
    /// <summary>End marker for TOC object section</summary>
    public const string ENDTOCOBJECT = "$$endTocObject";
    
    /// <summary>Start marker for example section</summary>
    public const string STARTEXAMPLE = "$$startExample";
    /// <summary>End marker for example section</summary>
    public const string ENDEXAMPLE = "$$endExample";
    #endregion

    /// <summary>
    /// Gets the raw filename without any manipulation applied.
    /// </summary>
    string RawFilename { get; }

    /// <summary>
    /// Gets the processed filename with naming rules applied if a handler is set.
    /// </summary>
    string ProcessedFilename { get; }

    /// <summary>
    /// Applies the variables to the provided content by replacing placeholders with actual values.
    /// The filename in the content will be replaced with the processed filename.
    /// </summary>
    /// <param name="content">The template content to process</param>
    /// <returns>The processed content with variables replaced</returns>
    string ApplyVariables(string content);

    /// <summary>
    /// Sets the handler for filename manipulation. This only affects how filenames are processed,
    /// not the content variables themselves.
    /// </summary>
    /// <param name="handler">The handler that implements naming manipulation rules for filenames</param>
    void SetFilenameHandler(INamingManipulationHandler handler);
}