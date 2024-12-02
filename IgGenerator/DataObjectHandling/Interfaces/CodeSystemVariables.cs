using Hl7.Fhir.Model;
using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling.Interfaces;

public class CodeSystemVariables(CodeSystem codeSystem) : IVariable
{
    private INamingManipulationHandler? _namingManipulationHandler;
    private string Name => codeSystem.Name;
    private string Url => codeSystem.Url;
    private string Filename => _namingManipulationHandler == null ? Name : _namingManipulationHandler.FilterPartFromFilename(Name);

    public string ApplyVariables(string content)
    {
        return content
            .Replace(IVariable.VARNAME_CS_NAME, Name)
            .Replace(IVariable.VARNAME_CS_URL, Url)
            .Replace(IVariable.VARNAME_FILENAME, Filename);
    }

    public void ApplyNamingManipulation(INamingManipulationHandler handler)
    {
        _namingManipulationHandler = handler;
    }
}