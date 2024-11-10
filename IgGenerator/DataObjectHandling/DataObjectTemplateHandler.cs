using System.Text.RegularExpressions;

namespace IgGenerator.DataObjectHandling;

public partial class DataObjectTemplateHandler : IDataObjectTemplateHandler
{
    private const string FolderName = "./IgTemplate/Einfuehrung/Datenobjekte/Datenobjekte_Template";

    private readonly Dictionary<string, string> _templates;

    public DataObjectTemplateHandler()
    {
        _templates = new Dictionary<string, string>();

        LoadTemplates();
    }

    private void LoadTemplates()
    {
        DirectoryInfo dir = new DirectoryInfo(FolderName);
        foreach (FileInfo enumerateFile in dir.EnumerateFiles())
        {
            _templates.Add(enumerateFile.Name, File.ReadAllText(enumerateFile.FullName));
        }
    }

    public IDictionary<string, string> ApplyVariables(IDataObjectVariables variables) =>
        _templates
            .ToDictionary(
                file => ApplyVariable(file.Key, variables), 
                file => file.Key.Contains("Beispiele")
                    ? HandleExample(file.Value, variables)
                    : ApplyVariable(file.Value, variables));

    private static string HandleExample(string content, IDataObjectVariables variables)
    {
        Match match = ExampleRegex().Match(content);
        string resultContent = ApplyVariable(content.Replace("\n" + match.Value, ""), variables);

        if (variables.ExampleNamesAndIds != null)
        {
            resultContent = variables.ExampleNamesAndIds
                .Select(example => match.Value
                    .Replace(IDataObjectVariables.VARNAME_EXAMPLENAME, example.Key)
                    .Replace(IDataObjectVariables.VARNAME_EXAMPLEID, example.Value))
                .Aggregate(resultContent, (current, exampleContent) => current + exampleContent);
        }
        
        return resultContent
            .Replace(IDataObjectVariables.STARTEXAMPLE,"")
            .Replace(IDataObjectVariables.ENDEXAMPLE,"");
    }

    private static string ApplyVariable(string input, IDataObjectVariables variables)
    {
        return input
            .Replace(IDataObjectVariables.VARNAME_RESOURCENAME, variables.ResourceName)
            .Replace(IDataObjectVariables.VARNAME_CANONICAL, variables.Canonical)
            .Replace(IDataObjectVariables.VARNAME_COREURL, variables.CoreUrl);
    }

    [GeneratedRegex(@"%igg\.startExample\s*(.*?)\s*%igg\.endExample", RegexOptions.Singleline)]
    private static partial Regex ExampleRegex();
}