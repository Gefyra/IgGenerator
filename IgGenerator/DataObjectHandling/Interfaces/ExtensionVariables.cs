using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling.Interfaces;

/// <summary>
/// Handles variables for FHIR Extensions, providing functionality to apply these variables
/// to templates and manage naming conventions.
/// </summary>
public class ExtensionVariables : IVariable
{
    private readonly string _name;
    private readonly string _url;
    private INamingManipulationHandler? _filenameHandler;

    /// <summary>
    /// Initializes a new instance of the ExtensionVariables class.
    /// </summary>
    /// <param name="name">The name of the extension</param>
    /// <param name="url">The URL of the extension</param>
    /// <exception cref="ArgumentNullException">Thrown when name or url is null</exception>
    public ExtensionVariables(string name, string url)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _url = url ?? throw new ArgumentNullException(nameof(url));
    }

    /// <inheritdoc />
    public string RawFilename => _name;

    /// <inheritdoc />
    public string ProcessedFilename => 
        _filenameHandler?.FilterPartFromFilename(RawFilename) ?? RawFilename;

    /// <summary>
    /// Applies the Extension variables to the provided template content.
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
            .Replace(IVariable.VARNAME_SD_NAME, _name)
            .Replace(IVariable.VARNAME_SD_URL, _url)
            .Replace(IVariable.VARNAME_FILENAME, ProcessedFilename);
    }

    /// <inheritdoc />
    public void SetFilenameHandler(INamingManipulationHandler handler)
    {
        _filenameHandler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public void ApplyNamingManipulation(INamingManipulationHandler handler)
    {
        throw new NotImplementedException();
    }
}