using IgGenerator.DataObjectHandling.Interfaces;

namespace IgGenerator.IgHandling;

public class TocFileManager : ITocFileManager
{
    private const string MISSING_TEMPLATE_ERROR = "No template found for type: {0}";
    
    private readonly Dictionary<Type, List<IVariable>> _registries;
    private readonly ITemplateHandler _templateHandler;

    public TocFileManager(ITemplateHandler templateHandler)
    {
        _templateHandler = templateHandler ?? throw new ArgumentNullException(nameof(templateHandler));
        
        _registries = new Dictionary<Type, List<IVariable>>
        {
            { typeof(StructureDefinitionVariables), new List<IVariable>() },
            { typeof(CodeSystemVariables), new List<IVariable>() },
            { typeof(ExtensionVariables), new List<IVariable>() },
            { typeof(CapabilityStatementVariables), new List<IVariable>() }
        };
    }

    public void RegisterVariable(IVariable variable)
    {
        Type variableType = variable.GetType();
        if (_registries.ContainsKey(variableType))
        {
            Console.WriteLine($"Registering {variableType.Name}");
            _registries[variableType].Add(variable);
        }
        else
        {
            Console.WriteLine($"Warning: Unsupported variable type {variableType.Name}");
        }
    }
    
    public string GetDataObjectTocFile() => GenerateTocFile(
        TemplateType.TocFile | TemplateType.DataObject,
        typeof(StructureDefinitionVariables));

    public string GetCodeSystemTocFile() => GenerateTocFile(
        TemplateType.TocFile | TemplateType.CodeSystem,
        typeof(CodeSystemVariables));

    public string GetExtensionTocFile() => GenerateTocFile(
        TemplateType.TocFile | TemplateType.Extension,
        typeof(ExtensionVariables));

    public string GetCapabilitySatementTocFile() => GenerateTocFile(
        TemplateType.TocFile | TemplateType.CapabilityStatement,
        typeof(CapabilityStatementVariables));

    private string GenerateTocFile(TemplateType templateType, Type registryType)
    {
        IEnumerable<Template> templates = _templateHandler.GetTemplate(templateType);
        Template template = templates.FirstOrDefault() 
            ?? throw new InvalidOperationException(string.Format(MISSING_TEMPLATE_ERROR, templateType));

        List<IVariable> registry = _registries[registryType];
        Console.WriteLine($"Generating TOC file for {registryType.Name} with {registry.Count} entries");
        
        return _templateHandler.ApplyTocList(template.Content, registry);
    }
}