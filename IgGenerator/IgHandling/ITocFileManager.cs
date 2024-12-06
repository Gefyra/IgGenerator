using IgGenerator.DataObjectHandling.Interfaces;

namespace IgGenerator.IgHandling;

public interface ITocFileManager
{
    public void RegisterVariable(IVariable dataObjectVariables);
    public string GetDataObjectTocFile();
    public string GetCodeSystemTocFile();
    public string GetExtensionTocFile();
    public string GetCapabilitySatementTocFile();
}