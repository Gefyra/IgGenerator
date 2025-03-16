namespace IgGenerator.DataObjectHandling.Interfaces;

/// <summary>
/// Defines the contract for handling templates in the implementation guide generation process.
/// This interface provides methods for applying variables to templates and managing template content.
/// </summary>
public interface ITemplateHandler
{
    /// <summary>
    /// Applies variables to data object templates and returns a dictionary of file names and their processed content.
    /// </summary>
    /// <param name="variables">The variables to apply to the templates</param>
    /// <returns>A dictionary mapping file names to their processed content</returns>
    IDictionary<string, string> ApplyDataObjectVariables(IVariable variables);

    /// <summary>
    /// Applies variables to a specific template type based on the variable type.
    /// </summary>
    /// <param name="variables">The variables to apply to the template</param>
    /// <returns>A key-value pair containing the processed file name and content</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported variable type is provided</exception>
    KeyValuePair<string, string> ApplyVariables(IVariable variables);

    /// <summary>
    /// Applies a list of variables to a TOC template content.
    /// </summary>
    /// <param name="content">The TOC template content</param>
    /// <param name="variables">The variables to apply to the TOC template</param>
    /// <returns>The processed TOC content with all variables replaced</returns>
    string ApplyTocList(string content, IEnumerable<IVariable> variables);

    /// <summary>
    /// Retrieves templates of a specific type.
    /// </summary>
    /// <param name="templateTypes">The type of templates to retrieve</param>
    /// <returns>An enumerable collection of matching templates</returns>
    IEnumerable<Template> GetTemplate(TemplateType templateTypes);
}