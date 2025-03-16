using Hl7.Fhir.Model;
using IgGenerator.Helpers;
using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling.Interfaces;

/// <summary>
/// Handles variables for FHIR CapabilityStatements, providing functionality to apply these variables
/// to templates and manage naming conventions.
/// </summary>
public class CapabilityStatementVariables : IVariable
{
    private readonly CapabilityStatement? _capabilityStatement;
    private INamingManipulationHandler? _filenameHandler;

    /// <summary>
    /// Initializes a new instance of the CapabilityStatementVariables class.
    /// </summary>
    /// <param name="capabilityStatement">The FHIR CapabilityStatement to handle</param>
    public CapabilityStatementVariables(CapabilityStatement? capabilityStatement)
    {
        _capabilityStatement = capabilityStatement;
    }

    private string Name => _capabilityStatement?.Name ?? string.Empty;
    private string Url => _capabilityStatement?.Url ?? string.Empty;

    /// <inheritdoc />
    public string RawFilename => $"Akteur-{Name}";

    /// <inheritdoc />
    public string ProcessedFilename => 
        _filenameHandler?.FilterPartFromFilename(RawFilename) ?? RawFilename;

    /// <summary>
    /// Applies the CapabilityStatement variables to the provided template content.
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
            .Replace(IVariable.VARNAME_CAPSTMT_NAME, Name)
            .Replace(IVariable.VARNAME_CAPSTMT_URL, Url)
            .Replace(IVariable.VARNAME_FILENAME, ProcessedFilename)
            .ReplaceVars();
    }

    /// <inheritdoc />
    public void SetFilenameHandler(INamingManipulationHandler handler)
    {
        _filenameHandler = handler ?? throw new ArgumentNullException(nameof(handler));
    }
}