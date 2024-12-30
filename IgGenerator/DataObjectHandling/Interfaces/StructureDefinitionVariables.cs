using System.Text.RegularExpressions;
using Hl7.Fhir.Model;
using IgGenerator.Helpers;
using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling.Interfaces;

public partial class StructureDefinitionVariables(StructureDefinition structureDefinition) : IVariable
{
    private INamingManipulationHandler? _namingManipulationHandler;
    private string Name => structureDefinition.Name;
    private string Url => structureDefinition.Url;
    private string Type => structureDefinition.Type;
    private string BaseUrl => structureDefinition.BaseDefinition;
    public IEnumerable<Resource>? Examples {get; set;}
    private string Filename =>
        _namingManipulationHandler == null
            ? LastPartOfCanonical().Match(structureDefinition.Url).Value
            : _namingManipulationHandler.FilterPartFromFilename(LastPartOfCanonical().Match(structureDefinition.Url).Value);

    public string ApplyVariables(string content)
    {
        string newContent = ApplyExamples(content);
        return newContent
            .Replace(IVariable.VARNAME_SD_NAME, Name)
            .Replace(IVariable.VARNAME_SD_URL, Url)
            .Replace(IVariable.VARNAME_SD_TYPE, Type)
            .Replace(IVariable.VARNAME_SD_BASEURL, BaseUrl.Contains("hl7.org") ? "": BaseUrl)
            .Replace(IVariable.VARNAME_FILENAME, Filename)
            .ReplaceVars();
    }
    
    private string ApplyExamples(string content)
    {
        Match match = ExampleRegex().Match(content);
        if (match.Success && Examples != null)
        {
            string resultContent = ApplyVariables(content.Replace("\n" + match.Value, ""));

            resultContent = Examples.Select(example => match.Value
                    .Replace(IVariable.VARNAME_R_ID, example.Id)) //TODO Extension Information
                .Aggregate(resultContent, (current, exampleContent) => current + exampleContent);

            return resultContent
                .Replace(IVariable.STARTEXAMPLE, "")
                .Replace(IVariable.ENDEXAMPLE, "");
        }
        return content;
    }
    
    [GeneratedRegex(@"\$\$startExample\s*(.*?)\s*\$\$endExample", RegexOptions.Singleline)]
    private static partial Regex ExampleRegex();
    
    [GeneratedRegex("[^/]+$")]
    private static partial Regex LastPartOfCanonical();

    public void ApplyNamingManipulation(INamingManipulationHandler handler)
    {
        _namingManipulationHandler = handler;
    }
}