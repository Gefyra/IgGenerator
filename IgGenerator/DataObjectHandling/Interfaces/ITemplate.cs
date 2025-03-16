namespace IgGenerator.DataObjectHandling.Interfaces;

/// <summary>
/// Represents a template with its type, file name, and content.
/// This immutable record is used to store and manage different types of templates
/// used in the implementation guide generation process.
/// </summary>
/// <param name="TemplateType">The type of the template, indicating its purpose and usage</param>
/// <param name="FileName">The name of the file containing the template</param>
/// <param name="Content">The actual content of the template</param>
public record Template(TemplateType TemplateType, string FileName, string Content);

/// <summary>
/// Defines the different types of templates used in the implementation guide.
/// This enum uses flag attributes to allow combining multiple template types.
/// </summary>
[Flags]
public enum TemplateType
{
    /// <summary>
    /// Template for standard data objects
    /// </summary>
    DataObject = 0,
    
    /// <summary>
    /// Template for code systems
    /// </summary>
    CodeSystem = 1,
    
    /// <summary>
    /// Template for FHIR extensions
    /// </summary>
    Extension = 2,
    
    /// <summary>
    /// Template for capability statements
    /// </summary>
    CapabilityStatement = 4,
    
    /// <summary>
    /// Template for table of contents files
    /// </summary>
    TocFile = 8,
    
    /// <summary>
    /// Template for copy-paste content
    /// </summary>
    CopyPasteFile = 16
}