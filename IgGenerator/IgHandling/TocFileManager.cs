using Hl7.Fhir.Language.Debugging;
using IgGenerator.DataObjectHandling;
using IgGenerator.DataObjectHandling.Interfaces;

namespace IgGenerator.IgHandling;

public class TocFileManager : ITocFileManager
{

    private readonly List<IVariable> _sdRegistry;
    private readonly List<IVariable> _codesystemRegistry;
    private readonly List<IVariable> _extensionRegistry;
    private readonly List<IVariable> _capabilityStatementRegistry;
    private readonly ITemplateHandler _templateHandler;

    public TocFileManager(ITemplateHandler templateHandler)
    {
        _templateHandler = templateHandler;
        
        _sdRegistry = [];
        _codesystemRegistry = [];
        _extensionRegistry = [];
        _capabilityStatementRegistry = [];
    }

    public void RegisterVariable(IVariable variable)
    {
        switch (variable)
        {
            case CodeSystemVariables: _codesystemRegistry.Add(variable);
                break;
            case ExtensionVariables:_extensionRegistry.Add(variable);
                break;
            case StructureDefinitionVariables: _sdRegistry.Add(variable);
                break;
            case CapabilityStatementVariables: _capabilityStatementRegistry.Add(variable);
                break;
        }
    }

    public string GetDataObjectTocFile() => GetTocFile(_templateHandler.DataObjectTocTemplate, _sdRegistry);

    public string GetCodeSystemTocFile() => GetTocFile(_templateHandler.CodeSystemTocTemplate, _codesystemRegistry);

    public string GetExtensionTocFile() => GetTocFile(_templateHandler.ExtensionTocTemplate, _extensionRegistry);
    public string GetCapabilitySatementTocFile() => GetTocFile(_templateHandler.CapabilityStatementTocTemplate, _capabilityStatementRegistry);

    private string GetTocFile(KeyValuePair<string, string> template, IEnumerable<IVariable> registry)
    {
        return _templateHandler.ApplyTocList(template.Value, registry);
    }
}