using System.Text.RegularExpressions;
using IG_Generator;

namespace IgGen;

public class DataObjectTemplateHandler
{
    private const string FolderName = "./IgTemplate/Einfuehrung/Datenobjekte/Datenobjekte_Template";

    private IDictionary<string, string> _templates;

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

    public IDictionary<string, string> ApplyVariables(IIgDataObjectVariables variables)
    {
        IDictionary<string, string> result = new Dictionary<string, string>();
        foreach (KeyValuePair<string, string> file in _templates)
        {
            result.Add(ApplyVariable(file.Key, variables),
                file.Key.Contains("Beispiele")
                    ? HandleExample(file.Value, variables)
                    : ApplyVariable(file.Value, variables));
        }

        return result;
    }

    private static string HandleExample(string content, IIgDataObjectVariables variables)
    {
        const string pattern = @"%igg\.startExample\s*(.*?)\s*%igg\.endExample";
        Match match = Regex.Match(content, pattern, RegexOptions.Singleline);

        string resultContent = ApplyVariable(content.Replace("\n" + match.Value, ""), variables);

        if (variables.ExampleNamesAndIds != null)
        {
            resultContent = variables.ExampleNamesAndIds
                .Select(example => match.Value
                    .Replace(IIgDataObjectVariables.VARNAME_EXAMPLENAME, example.Key)
                    .Replace(IIgDataObjectVariables.VARNAME_EXAMPLEID, example.Value))
                .Aggregate(resultContent, (current, exampleContent) => current + exampleContent);
        }
        
        return resultContent
            .Replace(IIgDataObjectVariables.STARTEXAMPLE,"")
            .Replace(IIgDataObjectVariables.ENDEXAMPLE,"");
    }

    private static string ApplyVariable(string input, IIgDataObjectVariables variables)
    {
        return input
            .Replace(IIgDataObjectVariables.VARNAME_RESOURCENAME, variables.ResourceName)
            .Replace(IIgDataObjectVariables.VARNAME_CANONICAL, variables.Canonical)
            .Replace(IIgDataObjectVariables.VARNAME_COREURL, variables.CoreUrl);
    }
}