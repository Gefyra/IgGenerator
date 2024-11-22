using System.Text.RegularExpressions;
using IgGenerator.Helpers;
using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling;

public partial class TemplateHandler : ITemplateHandler
{
    private const string DataObjectFolderName = "Datenobjekte";
    private const string DataObjectFolderPath = "./IgTemplate/Einfuehrung/Datenobjekte";
    private const string SingleDataObjectTemplateFolderName = $"{DataObjectFolderPath}/Datenobjekte_Template";
    private const string CodeSystemTemplateFolder = $"{DataObjectFolderPath}/Terminologien/";
    private const string ExtensionTemplateFolder = $"{DataObjectFolderPath}/Extensions/";

    private readonly Dictionary<string, string> _dataObjectTemplates;
    private KeyValuePair<string, string> _codeSystemTemplate;
    private KeyValuePair<string, string> _extensionTemplate;
    private readonly INamingManipulationHandler _namingManipulationHandler;
    public KeyValuePair<string, string> DataObjectTocTemplate { get; private set; }
    public KeyValuePair<string, string> CodeSystemTocTemplate { get; private set; }
    public KeyValuePair<string, string> ExtensionTocTemplate { get; private set; }

    public Dictionary<string, string> CopyPasteFiles { get; init; }
    public Dictionary<string, string> TerminologyFixFiles { get; init; }

    public TemplateHandler(INamingManipulationHandler namingManipulationHandler)
    {
        _namingManipulationHandler = namingManipulationHandler;
        _dataObjectTemplates = new Dictionary<string, string>();
        CopyPasteFiles = new Dictionary<string, string>();
        TerminologyFixFiles = new Dictionary<string, string>();

        LoadSingleDataObjectTemplates();
        LoadCodeSystemTemplate();
        LoadExtensionTemplate();
        LoadCopyPasteFiles();
        LoadTerminologyFixFiles();
        LoadTocTemplates();
    }

    private void LoadTocTemplates()
    {
        DirectoryInfo dir = new(DataObjectFolderPath);
        FileInfo doTocTemplate = dir.EnumerateFiles("*.yaml", SearchOption.TopDirectoryOnly).First();
        DataObjectTocTemplate = new KeyValuePair<string, string>("toc.yaml", File.ReadAllText(doTocTemplate.FullName));

        DirectoryInfo dirCs = new(CodeSystemTemplateFolder)!;
        FileInfo csTocTemplate = dirCs.EnumerateFiles("*.yaml", SearchOption.TopDirectoryOnly).First();
        CodeSystemTocTemplate = new KeyValuePair<string, string>("toc.yaml", File.ReadAllText(csTocTemplate.FullName));
        
        DirectoryInfo dirEx = new(ExtensionTemplateFolder)!;
        FileInfo exTocTemplate = dirEx.EnumerateFiles("*.yaml", SearchOption.TopDirectoryOnly).First();
        ExtensionTocTemplate = new KeyValuePair<string, string>("toc.yaml", File.ReadAllText(exTocTemplate.FullName));
    }

    private void LoadCopyPasteFiles()
    {
        DirectoryInfo dir = new(DataObjectFolderPath);
        foreach (FileInfo fileInfo in dir.EnumerateFiles("*.md", SearchOption.AllDirectories))
        {
            if (!fileInfo.Name.Contains("%igg"))
            {
                CopyPasteFiles.Add(fileInfo.FullName.GetPathFromFolder(DataObjectFolderName)!, File.ReadAllText(fileInfo.FullName));
            }
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
        _codeSystemTemplate = new KeyValuePair<string, string>(file!.Name, File.ReadAllText(file.FullName));
    }
    
    private void LoadExtensionTemplate()
    {
        DirectoryInfo dir = new(ExtensionTemplateFolder);
        FileInfo? file = dir.EnumerateFiles().FirstOrDefault(e => e.Name.StartsWith("%igg.resourceName.page.md"));
        _extensionTemplate = new KeyValuePair<string, string>(file!.Name, File.ReadAllText(file.FullName));
    }

    public IDictionary<string, string> ApplyProfileVariables(IDataObjectVariables variables) =>
        _dataObjectTemplates
            .ToDictionary(
                file => ApplyVariable(file.Key, variables), 
                file => file.Key.Contains("Beispiele")
                    ? HandleExample(file.Value, variables)
                    : ApplyVariable(file.Value, variables));
    
    public KeyValuePair<string, string> ApplyTermVariables(IDataObjectTerminologyVariables variables) =>
        new(
            ApplyVariable(_codeSystemTemplate.Key, variables),
            ApplyVariable(_codeSystemTemplate.Value, variables));
    
    public KeyValuePair<string, string> ApplyExtensionVariables(IDataObjectVariables variables) =>
        new(
            ApplyVariable(_extensionTemplate.Key, variables),
            ApplyVariable(_extensionTemplate.Value, variables));

    private string HandleExample(string content, IDataObjectVariables variables)
    {
        Match match = ExampleRegex().Match(content);
        string resultContent = ApplyVariable(content.Replace("\n" + match.Value, ""), variables);

        if (variables.ExampleNamesAndIds != null)
        {
            resultContent = variables.ExampleNamesAndIds
                .Select(example => match.Value
                    .Replace(IVariable.VARNAME_EXAMPLENAME, example.Key)
                    .Replace(IVariable.VARNAME_EXAMPLEID, example.Value))
                .Aggregate(resultContent, (current, exampleContent) => current + exampleContent);
        }
        
        return resultContent
            .Replace(IVariable.STARTEXAMPLE,"")
            .Replace(IVariable.ENDEXAMPLE,"");
    }
    
    public string ApplyTocList(string content, IEnumerable<IVariable> variables)
    {
        Match match = TocObjectRegex().Match(content);
        IEnumerable<string> allTocObjects = variables.Select(e=>ApplyVariable(match.Value, e));

        string result = allTocObjects.Aggregate(content, (current, exampleContent) => current + exampleContent);
        
        return result
            .Replace(match.Value, "")
            .Replace(IVariable.STARTTOCOBJECT,"")
            .Replace(IVariable.ENDTOCOBJECT,"");
    }

    private string ApplyVariable(string input, IVariable variables)
    {
        return variables switch
        {
            IDataObjectVariables variable => input
                .Replace(IVariable.VARNAME_CANONICAL, variable.Canonical)
                .Replace(IVariable.VARNAME_COREURL, variable.CoreUrl)
                .Replace(IVariable.VARNAME_RESOURCENAME, variable.ResourceName)
                .Replace(IVariable.VARNAME_FILENAME_UMLAUT, _namingManipulationHandler.FilterPartFromFilename(variable.GetFilename().ChangeUmlaut()))
                .Replace(IVariable.VARNAME_FILENAME, _namingManipulationHandler.FilterPartFromFilename(variable.GetFilename()))
                .Replace(IVariable.STARTTOCOBJECT, "")
                .Replace(IVariable.ENDTOCOBJECT, "")
                .Replace("%igg", ""),
            IDataObjectTerminologyVariables variable => input
                .Replace(IVariable.VARNAME_TERMNAME, variable.TerminologyName)
                .Replace(IVariable.VARNAME_CANONICAL, variable.Canonical)
                .Replace(IVariable.STARTTOCOBJECT, "")
                .Replace(IVariable.ENDTOCOBJECT, "")
                .Replace("%igg", ""),
            _ => string.Empty
        };
    }

    [GeneratedRegex(@"%igg\.startExample\s*(.*?)\s*%igg\.endExample", RegexOptions.Singleline)]
    private static partial Regex ExampleRegex();
    
    [GeneratedRegex(@"%igg\.startTocObject\s*(.*?)\s*%igg\.endTocObject", RegexOptions.Singleline)]
    private static partial Regex TocObjectRegex();
}