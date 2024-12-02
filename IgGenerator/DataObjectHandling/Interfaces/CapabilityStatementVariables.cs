using Hl7.Fhir.Model;
using IgGenerator.Helpers;
using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling.Interfaces;

public class CapabilityStatementVariables(CapabilityStatement capabilityStatement) : IVariable
{
    private INamingManipulationHandler? _namingManipulationHandler;
    private string Name => capabilityStatement.Name;
    private string Url => capabilityStatement.Url;
    private string Filename
    {
        get
        {
            string filename = $"Akteur-{capabilityStatement.Name}";
            return _namingManipulationHandler == null
                ? filename
                : _namingManipulationHandler.FilterPartFromFilename(filename);
        }
    }

    public string ApplyVariables(string content)
    {
        return content
            .Replace(IVariable.VARNAME_CAPSTMT_NAME, Name)
            .Replace(IVariable.VARNAME_CAPSTMT_URL, Url)
            .Replace(IVariable.VARNAME_FILENAME, Filename)
            .ReplaceVars();
    }

    public void ApplyNamingManipulation(INamingManipulationHandler handler)
    {
        _namingManipulationHandler = handler;
    }
}