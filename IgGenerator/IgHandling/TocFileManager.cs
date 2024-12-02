using IgGenerator.DataObjectHandling;
using IgGenerator.DataObjectHandling.Interfaces;

namespace IgGenerator.IgHandling;

public class TocFileManager : ITocFileManager
{

    private readonly List<IVariable> _dataObjectRegistry;
    private readonly List<IVariable> _codesystemRegistry;
    private readonly List<IVariable> _extensionRegistry;
    private readonly ITemplateHandler _templateHandler;

    public TocFileManager(ITemplateHandler templateHandler)
    {
        _templateHandler = templateHandler;
        
        _dataObjectRegistry = new List<IVariable>();
        _codesystemRegistry = new List<IVariable>();
        _extensionRegistry = new List<IVariable>();
    }

    public void RegisterDataObject(IVariable dataObjectVariables)
    {
        _dataObjectRegistry.Add(dataObjectVariables);
    }

    public void RegisterCodesystem(IVariable terminologyVariables)
    {
        _codesystemRegistry.Add(terminologyVariables);
    }
    
    public void RegisterExtension(IVariable dataObjectVariables)
    {
        _extensionRegistry.Add(dataObjectVariables);
    }

    public string GetDataObjectTocFile() => GetTocFile(_templateHandler.DataObjectTocTemplate, _dataObjectRegistry);

    public string GetCodeSystemTocFile() => GetTocFile(_templateHandler.CodeSystemTocTemplate, _codesystemRegistry);

    public string GetExtensionTocFile() => GetTocFile(_templateHandler.ExtensionTocTemplate, _extensionRegistry);

    private string GetTocFile(KeyValuePair<string, string> template, IEnumerable<IVariable> registry)
    {
        return _templateHandler.ApplyTocList(template.Value, registry);
    }
}