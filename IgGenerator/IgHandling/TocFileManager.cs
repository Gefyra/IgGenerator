using IgGenerator.DataObjectHandling;

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

    public void RegisterDataObject(IDataObjectVariables dataObjectVariables)
    {
        _dataObjectRegistry.Add(dataObjectVariables);
    }

    public void RegisterCodesystem(IDataObjectTerminologyVariables terminologyVariables)
    {
        _codesystemRegistry.Add(terminologyVariables);
    }
    
    public void RegisterExtension(IDataObjectVariables dataObjectVariables)
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

public interface ITocFileManager
{
    public void RegisterDataObject(IDataObjectVariables dataObjectVariables);
    public void RegisterCodesystem(IDataObjectTerminologyVariables terminologyVariables);
    public void RegisterExtension(IDataObjectVariables dataObjectVariables);
    public string GetDataObjectTocFile();
    public string GetCodeSystemTocFile();
    public string GetExtensionTocFile();

}