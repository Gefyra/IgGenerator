using IgGenerator.DataObjectHandling.Interfaces;

namespace IgGenerator.IgHandling;

public interface ITocFileManager
{
    public void RegisterDataObject(IVariable dataObjectVariables);
    public void RegisterCodesystem(IVariable terminologyVariables);
    public void RegisterExtension(IVariable dataObjectVariables);
    public string GetDataObjectTocFile();
    public string GetCodeSystemTocFile();
    public string GetExtensionTocFile();

}