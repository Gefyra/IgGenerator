using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling.Interfaces;

public class ExtensionVariables(string name, string url) : IVariable
{
    private string Name => name;
    private string Url => url;
    private string Filename => name;

    public string ApplyVariables(string content)
    {
        return content
            .Replace(IVariable.VARNAME_SD_NAME, Name)
            .Replace(IVariable.VARNAME_SD_URL, Url)
            .Replace(IVariable.VARNAME_FILENAME, Filename);
    }

    public void ApplyNamingManipulation(INamingManipulationHandler handler)
    {
        throw new NotImplementedException();
    }
}