using System.Text.RegularExpressions;

namespace IgGenerator.DataObjectHandling;

public partial class DataObjectTemplateHandler : IDataObjectTemplateHandler
{
    private const string DataObjectFolderName = "./IgTemplate/Einfuehrung/Datenobjekte";
    private const string SingleDataObjectTemplateFolderName = $"{DataObjectFolderName}/Datenobjekte_Template";
    private const string CodeSystemTemplateFolder = $"{DataObjectFolderName}/Terminologien/";

    private readonly Dictionary<string, string> _dataObjectTemplates;
    private KeyValuePair<string, string> _codeSystemTemplate;
    
    public Dictionary<string, string> TopLevelDataObjectFixFiles { get; init; }
    public Dictionary<string, string> TerminologyFixFiles { get; init; }

    public DataObjectTemplateHandler()
    {
        _dataObjectTemplates = new Dictionary<string, string>();
        TopLevelDataObjectFixFiles = new Dictionary<string, string>();
        TerminologyFixFiles = new Dictionary<string, string>();

        LoadSingleDataObjectTemplates();
        LoadCodeSystemTemplate();
        LoadFixedFiles();
        LoadTerminologyFixFiles();
    }

    private void LoadFixedFiles()
    {
        DirectoryInfo dir = new(DataObjectFolderName);
        foreach (FileInfo fileInfo in dir.EnumerateFiles("*.md", SearchOption.TopDirectoryOnly))
        {
            TopLevelDataObjectFixFiles.Add(fileInfo.Name, File.ReadAllText(fileInfo.FullName));
        }
    }

    private void LoadSingleDataObjectTemplates()
    {
        DirectoryInfo dir = new(SingleDataObjectTemplateFolderName);
        foreach (FileInfo enumerateFile in dir.EnumerateFiles())
        {
            _dataObjectTemplates.Add(enumerateFile.Name, File.ReadAllText(enumerateFile.FullName));
        }
    }

    private void LoadTerminologyFixFiles()
    {
        DirectoryInfo termDir = new(CodeSystemTemplateFolder);
        foreach (FileInfo enumerateFile in termDir.EnumerateFiles().Where(e=>!e.Name.Contains("%igg.")))
        {
            TerminologyFixFiles.Add(enumerateFile.Name, File.ReadAllText(enumerateFile.FullName));
        }
    }

    private void LoadCodeSystemTemplate()
    {
        DirectoryInfo dir = new(CodeSystemTemplateFolder);
        FileInfo? file = dir.EnumerateFiles().FirstOrDefault(e => e.Name.StartsWith("CodeSystem"));
        _codeSystemTemplate = new KeyValuePair<string, string>(file.Name, File.ReadAllText(file.FullName));
    }

    public IDictionary<string, string> ApplyVariables(IDataObjectVariables variables) =>
        _dataObjectTemplates
            .ToDictionary(
                file => ApplyVariable(file.Key, variables), 
                file => file.Key.Contains("Beispiele")
                    ? HandleExample(file.Value, variables)
                    : ApplyVariable(file.Value, variables));
    
    public KeyValuePair<string, string> ApplyVariables(IDataObjectTerminologyVariables variables) =>
        new(
            ApplyVariable(_codeSystemTemplate.Key, variables),
            ApplyVariable(_codeSystemTemplate.Value, variables));

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

    private static string ApplyVariable(string input, IDataObjectVariables variables) =>
        input
            .Replace(IDataObjectVariables.VARNAME_RESOURCENAME, variables.ResourceName)
            .Replace(IDataObjectVariables.VARNAME_CANONICAL, variables.Canonical)
            .Replace(IDataObjectVariables.VARNAME_COREURL, variables.CoreUrl);

    private static string ApplyVariable(string input, IDataObjectTerminologyVariables variables) =>
        input
            .Replace(IDataObjectTerminologyVariables.VARNAME_TERMNAME, variables.TerminologyName)
            .Replace(IDataObjectTerminologyVariables.VARNAME_CANONICAL, variables.Canonical);

    [GeneratedRegex(@"%igg\.startExample\s*(.*?)\s*%igg\.endExample", RegexOptions.Singleline)]
    private static partial Regex ExampleRegex();
}