using Hl7.Fhir.Model;
using IgGenerator.Helpers;
using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling.Interfaces;

public class StructureDefinitionVariables(StructureDefinition structureDefinition) : IVariable
{
    private INamingManipulationHandler? _namingManipulationHandler;
    private string Name => structureDefinition.Name;
    private string Url => structureDefinition.Url;
    private string Type => structureDefinition.Type;
    private string BaseUrl => structureDefinition.BaseDefinition;
    private string Filename =>
        _namingManipulationHandler == null
            ? structureDefinition.Name
            : _namingManipulationHandler.FilterPartFromFilename(structureDefinition.Name);

    public string ApplyVariables(string content)
    {
        return content
            .Replace(IVariable.VARNAME_SD_NAME, Name)
            .Replace(IVariable.VARNAME_SD_URL, Url)
            .Replace(IVariable.VARNAME_SD_TYPE, Type)
            .Replace(IVariable.VARNAME_SD_BASEURL, BaseUrl.Contains("hl7.org") ? "": BaseUrl)
            .Replace(IVariable.VARNAME_FILENAME, Filename)
            .ReplaceVars();
    }

    public void ApplyNamingManipulation(INamingManipulationHandler handler)
    {
        _namingManipulationHandler = handler;
    }
}