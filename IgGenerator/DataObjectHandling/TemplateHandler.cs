using System.Text.RegularExpressions;
using IgGenerator.DataObjectHandling.Interfaces;
using IgGenerator.DataObjectHandling.Services;
using IgGenerator.Helpers;
using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling;

/// <summary>
/// Main handler class for managing and processing templates in the Implementation Guide generation process.
/// Acts as a facade for template loading and processing services.
/// </summary>
public class TemplateHandler : ITemplateHandler
{
    private readonly TemplateLoadingService _loadingService;
    private readonly TemplateProcessingService _processingService;
    
    /// <summary>
    /// Initializes a new instance of the TemplateHandler class.
    /// </summary>
    /// <param name="namingManipulationHandler">Handler for filename manipulation</param>
    /// <exception cref="ArgumentNullException">Thrown when namingManipulationHandler is null</exception>
    public TemplateHandler(INamingManipulationHandler namingManipulationHandler)
    {
        if (namingManipulationHandler == null)
            throw new ArgumentNullException(nameof(namingManipulationHandler));

        _loadingService = new TemplateLoadingService();
        var templates = _loadingService.LoadAllTemplates().ToList();
        _processingService = new TemplateProcessingService(namingManipulationHandler, templates);
    }

    /// <inheritdoc />
    public KeyValuePair<string, string> ApplyVariables(IVariable variables) => 
        _processingService.ApplyVariables(variables);

    /// <inheritdoc />
    public IDictionary<string, string> ApplyDataObjectVariables(IVariable variables) => 
        _processingService.ApplyDataObjectVariables(variables);

    /// <inheritdoc />
    public string ApplyTocList(string content, IEnumerable<IVariable> variables) => 
        _processingService.ApplyTocList(content, variables);

    /// <inheritdoc />
    public IEnumerable<Template> GetTemplate(TemplateType templateTypes) => 
        _processingService.GetTemplate(templateTypes);
}