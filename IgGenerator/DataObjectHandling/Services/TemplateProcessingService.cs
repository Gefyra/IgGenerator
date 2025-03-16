using System.Text.RegularExpressions;
using IgGenerator.DataObjectHandling.Interfaces;
using IgGenerator.Helpers;
using IgGenerator.IgHandling;
using System.Text;

namespace IgGenerator.DataObjectHandling.Services;

/// <summary>
/// Service responsible for processing templates and applying variables.
/// </summary>
public partial class TemplateProcessingService
{
    private readonly INamingManipulationHandler _namingManipulationHandler;
    private readonly IEnumerable<Template> _templates;

    /// <summary>
    /// Initializes a new instance of the TemplateProcessingService class.
    /// </summary>
    /// <param name="namingManipulationHandler">Handler for filename manipulation</param>
    /// <param name="templates">Collection of templates to process</param>
    /// <exception cref="ArgumentNullException">Thrown when namingManipulationHandler or templates is null</exception>
    public TemplateProcessingService(INamingManipulationHandler namingManipulationHandler, IEnumerable<Template> templates)
    {
        _namingManipulationHandler = namingManipulationHandler ?? throw new ArgumentNullException(nameof(namingManipulationHandler));
        _templates = templates ?? throw new ArgumentNullException(nameof(templates));
    }

    /// <summary>
    /// Applies variables to a template based on the variable type.
    /// </summary>
    /// <param name="variables">The variables to apply</param>
    /// <returns>A key-value pair containing the processed filename and content</returns>
    /// <exception cref="ArgumentNullException">Thrown when variables is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when variable type is not supported</exception>
    /// <exception cref="InvalidOperationException">Thrown when template processing fails</exception>
    public KeyValuePair<string, string> ApplyVariables(IVariable variables)
    {
        if (variables == null)
            throw new ArgumentNullException(nameof(variables));

        try
        {
            variables.SetFilenameHandler(_namingManipulationHandler);
            return variables switch
            {
                CodeSystemVariables => ApplyVariablesToTemplate(variables, TemplateType.CodeSystem),
                ExtensionVariables => ApplyVariablesToTemplate(variables, TemplateType.Extension),
                CapabilityStatementVariables => ApplyVariablesToTemplate(variables, TemplateType.CapabilityStatement),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(variables), 
                    variables.GetType().Name, 
                    "Unsupported variable type. Only CodeSystem, Extension, and CapabilityStatement variables are supported.")
            };
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            var error = $"Failed to apply variables of type {variables.GetType().Name}: {ex.Message}";
            Console.WriteLine(error);
            throw new InvalidOperationException(error, ex);
        }
    }

    /// <summary>
    /// Applies variables to data object templates.
    /// </summary>
    /// <param name="variables">The variables to apply</param>
    /// <returns>A dictionary containing processed filenames and their content</returns>
    /// <exception cref="ArgumentNullException">Thrown when variables is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when no data object templates are found or processing fails</exception>
    public IDictionary<string, string> ApplyDataObjectVariables(IVariable variables)
    {
        if (variables == null)
            throw new ArgumentNullException(nameof(variables));

        try
        {
            variables.SetFilenameHandler(_namingManipulationHandler);
            var dataObjectTemplates = GetTemplatesOfType(TemplateType.DataObject).ToList();
            
            if (!dataObjectTemplates.Any())
            {
                throw new InvalidOperationException("No data object templates found");
            }

            var result = new Dictionary<string, string>();
            Console.WriteLine($"Processing {dataObjectTemplates.Count} data object templates...");

            foreach (Template template in dataObjectTemplates)
            {
                try
                {
                    // Ersetze die Variablen im Template-Dateinamen
                    string fileName = variables.ApplyVariables(template.FileName);
                    string content = variables.ApplyVariables(template.Content);
                    
                    result.Add(fileName, content);
                    Console.WriteLine($"Successfully processed template: {template.FileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to process template {template.FileName}: {ex.Message}");
                    // Continue processing other templates
                }
            }

            if (!result.Any())
            {
                throw new InvalidOperationException("Failed to process any data object templates");
            }

            Console.WriteLine($"Successfully processed {result.Count} data object templates");
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException && ex.Message != "No data object templates found")
        {
            var error = $"Failed to process data object templates: {ex.Message}";
            Console.WriteLine(error);
            throw new InvalidOperationException(error, ex);
        }
    }

    /// <summary>
    /// Applies a list of variables to TOC content.
    /// </summary>
    /// <param name="content">The TOC content to process</param>
    /// <param name="variables">The variables to apply</param>
    /// <returns>The processed TOC content</returns>
    /// <exception cref="ArgumentNullException">Thrown when content or variables is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when TOC processing fails</exception>
    public string ApplyTocList(string content, IEnumerable<IVariable> variables)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));
        if (variables == null)
            throw new ArgumentNullException(nameof(variables));

        try
        {
            var variablesList = variables.ToList();
            if (!variablesList.Any())
            {
                Console.WriteLine("Warning: No variables provided for TOC processing");
                // Remove all TOC blocks when no variables are present
                return TocObjectRegex().Replace(content, "");
            }

            // Process all matches
            string result = content;
            var matches = TocObjectRegex().Matches(content).ToList();
            if (!matches.Any())
            {
                Console.WriteLine("Warning: No TOC object pattern found in content");
                return content;
            }

            Console.WriteLine($"Processing TOC with {variablesList.Count} variables...");
            
            // Set filename handlers for all variables
            foreach (var variable in variablesList.Where(v => v != null))
            {
                variable.SetFilenameHandler(_namingManipulationHandler);
            }

            // Process each match in reverse order to maintain correct indices
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                var match = matches[i];
                string templateContent = match.Groups[1].Value.Trim();
                var processedEntries = new List<string>();

                foreach (var variable in variablesList.Where(v => v != null))
                {
                    try
                    {
                        var processed = variable.ApplyVariables(templateContent);
                        if (!string.IsNullOrWhiteSpace(processed))
                        {
                            processedEntries.Add(processed.Trim());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Failed to process TOC entry for variable type {variable.GetType().Name}: {ex.Message}");
                    }
                }

                // Always remove the block
                result = result.Remove(match.Index, match.Length);
                
                // Insert processed entries if any
                if (processedEntries.Any())
                {
                    result = result.Insert(match.Index, string.Join(Environment.NewLine, processedEntries));
                    Console.WriteLine($"Successfully processed {processedEntries.Count} TOC entries");
                }
                else
                {
                    Console.WriteLine("No TOC entries were processed, removing template block");
                }
            }

            return result.ReplaceVars();
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            var error = $"Failed to process TOC content: {ex.Message}";
            Console.WriteLine(error);
            throw new InvalidOperationException(error, ex);
        }
    }

    /// <summary>
    /// Gets templates of a specific type.
    /// </summary>
    /// <param name="templateTypes">The type of templates to retrieve</param>
    /// <returns>A collection of matching templates</returns>
    /// <exception cref="InvalidOperationException">Thrown when no templates are found for the specified type</exception>
    public IEnumerable<Template> GetTemplate(TemplateType templateTypes)
    {
        var templates = GetTemplatesOfType(templateTypes).ToList();
        
        if (!templates.Any())
        {
            var error = $"No templates found for type: {templateTypes}";
            Console.WriteLine(error);
            throw new InvalidOperationException(error);
        }

        return templates;
    }

    /// <summary>
    /// Gets a single template of the specified type.
    /// </summary>
    /// <param name="templateType">The type of template to retrieve</param>
    /// <returns>The first matching template</returns>
    /// <exception cref="InvalidOperationException">Thrown when no template is found for the specified type</exception>
    private Template GetTemplateOfType(TemplateType templateType)
    {
        var template = _templates.FirstOrDefault(t => t.TemplateType == templateType);
        
        if (template == null)
        {
            var error = $"No template found for type: {templateType}";
            Console.WriteLine(error);
            throw new InvalidOperationException(error);
        }

        return template;
    }

    /// <summary>
    /// Gets all templates of the specified type.
    /// </summary>
    /// <param name="templateType">The type of templates to retrieve</param>
    /// <returns>A collection of matching templates</returns>
    private IEnumerable<Template> GetTemplatesOfType(TemplateType templateType)
    {
        if (!_templates.Any())
        {
            Console.WriteLine("Warning: Template collection is empty");
        }
        
        return _templates.Where(t => t.TemplateType == templateType);
    }

    private KeyValuePair<string, string> ApplyVariablesToTemplate(IVariable variables, TemplateType templateType)
    {
        Template template = GetTemplateOfType(templateType);
        return new KeyValuePair<string, string>(
            variables.ApplyVariables(template.FileName),
            variables.ApplyVariables(template.Content));
    }

    [GeneratedRegex(@"\$\$startTocObject\s*(.*?)\s*\$\$endTocObject", RegexOptions.Singleline)]
    private static partial Regex TocObjectRegex();
} 