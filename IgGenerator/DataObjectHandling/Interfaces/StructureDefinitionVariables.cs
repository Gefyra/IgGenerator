using System.Text.RegularExpressions;
using Hl7.Fhir.Model;
using IgGenerator.Helpers;
using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling.Interfaces;

/// <summary>
/// Handles variables for FHIR Structure Definitions, providing functionality to apply these variables
/// to templates and manage naming conventions.
/// </summary>
public partial class StructureDefinitionVariables : IVariable
{
    private readonly StructureDefinition _structureDefinition;
    private INamingManipulationHandler? _filenameHandler;

    /// <summary>
    /// Gets the collection of example resources associated with this Structure Definition.
    /// </summary>
    public IEnumerable<Resource>? Examples { get; set; }

    /// <summary>
    /// Initializes a new instance of the StructureDefinitionVariables class.
    /// </summary>
    /// <param name="structureDefinition">The FHIR Structure Definition to handle</param>
    /// <exception cref="ArgumentNullException">Thrown when structureDefinition is null</exception>
    public StructureDefinitionVariables(StructureDefinition structureDefinition)
    {
        _structureDefinition = structureDefinition ?? throw new ArgumentNullException(nameof(structureDefinition));
    }

    private string Name => _structureDefinition.Name;
    private string Url => _structureDefinition.Url;
    private string Type => _structureDefinition.Type;
    private string BaseUrl => _structureDefinition.BaseDefinition;

    /// <inheritdoc />
    public string RawFilename => LastPartOfCanonical().Match(_structureDefinition.Url).Value;

    /// <inheritdoc />
    public string ProcessedFilename => 
        _filenameHandler?.FilterPartFromFilename(RawFilename) ?? RawFilename;

    /// <summary>
    /// Applies the Structure Definition variables to the provided template content.
    /// </summary>
    /// <param name="content">The template content to process</param>
    /// <returns>The processed content with all variables replaced</returns>
    public string ApplyVariables(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        string newContent = ApplyExamples(content);
        return newContent
            .Replace(IVariable.VARNAME_SD_NAME, Name)
            .Replace(IVariable.VARNAME_SD_URL, Url)
            .Replace(IVariable.VARNAME_SD_TYPE, Type)
            .Replace(IVariable.VARNAME_SD_BASEURL, BaseUrl.Contains("hl7.org") ? "" : BaseUrl)
            .Replace(IVariable.VARNAME_FILENAME, ProcessedFilename)
            .ReplaceVars();
    }
    
    /// <summary>
    /// Applies example resources to the template content if available.
    /// </summary>
    /// <param name="content">The template content to process</param>
    /// <returns>The processed content with example variables replaced</returns>
    private string ApplyExamples(string content)
    {
        Match match = ExampleRegex().Match(content);
        if (!match.Success || Examples == null)
        {
            return content;
        }

        string resultContent = ApplyVariables(content.Replace("\n" + match.Value, ""));

        resultContent = Examples
            .Select(example => match.Value.Replace(IVariable.VARNAME_R_ID, example.Id ?? string.Empty))
            .Aggregate(resultContent, (current, exampleContent) => current + exampleContent);

        return resultContent
            .Replace(IVariable.STARTEXAMPLE, "")
            .Replace(IVariable.ENDEXAMPLE, "");
    }

    /// <inheritdoc />
    public void SetFilenameHandler(INamingManipulationHandler handler)
    {
        _filenameHandler = handler ?? throw new ArgumentNullException(nameof(handler));
    }
    
    [GeneratedRegex(@"\$\$startExample\s*(.*?)\s*\$\$endExample", RegexOptions.Singleline)]
    private static partial Regex ExampleRegex();
    
    [GeneratedRegex("[^/]+$")]
    private static partial Regex LastPartOfCanonical();
}