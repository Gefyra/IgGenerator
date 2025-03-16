using Hl7.Fhir.Model;
using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling.Interfaces;

/// <summary>
/// Handles variables for FHIR CodeSystems, providing functionality to apply these variables
/// to templates and manage naming conventions.
/// </summary>
public class CodeSystemVariables : IVariable
{
    private readonly CodeSystem _codeSystem;
    private INamingManipulationHandler? _filenameHandler;

    /// <summary>
    /// Initializes a new instance of the CodeSystemVariables class.
    /// </summary>
    /// <param name="codeSystem">The FHIR CodeSystem to handle</param>
    /// <exception cref="ArgumentNullException">Thrown when codeSystem is null</exception>
    public CodeSystemVariables(CodeSystem codeSystem)
    {
        _codeSystem = codeSystem ?? throw new ArgumentNullException(nameof(codeSystem));
    }

    private string Name => _codeSystem.Name;
    private string Url => _codeSystem.Url;

    /// <inheritdoc />
    public string RawFilename => Name;

    /// <inheritdoc />
    public string ProcessedFilename => 
        _filenameHandler?.FilterPartFromFilename(RawFilename) ?? RawFilename;

    /// <summary>
    /// Applies the CodeSystem variables to the provided template content.
    /// </summary>
    /// <param name="content">The template content to process</param>
    /// <returns>The processed content with all variables replaced</returns>
    public string ApplyVariables(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        return content
            .Replace(IVariable.VARNAME_CS_NAME, Name)
            .Replace(IVariable.VARNAME_CS_URL, Url)
            .Replace(IVariable.VARNAME_FILENAME, ProcessedFilename);
    }

    /// <inheritdoc />
    public void SetFilenameHandler(INamingManipulationHandler handler)
    {
        _filenameHandler = handler ?? throw new ArgumentNullException(nameof(handler));
    }
}