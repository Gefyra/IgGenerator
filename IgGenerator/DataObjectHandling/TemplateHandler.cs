using System.Text.RegularExpressions;
using Hl7.Fhir.Model;
using IgGenerator.DataObjectHandling.Interfaces;
using IgGenerator.Helpers;
using IgGenerator.IgHandling;

namespace IgGenerator.DataObjectHandling;

public partial class TemplateHandler : ITemplateHandler
{
    private const string DataObjectFolderName = "Datenobjekte";
    private const string DataObjectFolderPath = "./IgTemplate/Home/Datenobjekte";
    private const string SingleDataObjectTemplateFolderName = $"{DataObjectFolderPath}/Datenobjekte_Template";
    private const string CodeSystemTemplateFolder = $"{DataObjectFolderPath}/Terminologien/";
    private const string ExtensionTemplateFolder = $"{DataObjectFolderPath}/Extensions/";
    private const string CapabilityStatementFolder = $"{DataObjectFolderPath}/CapabilityStatements/";

    private readonly Dictionary<string, string> _dataObjectTemplates;
    private KeyValuePair<string, string> _codeSystemTemplate;
    private KeyValuePair<string, string> _extensionTemplate;
    private readonly INamingManipulationHandler _namingManipulationHandler;
    private KeyValuePair<string,string> _capabilityStatementTemplate;
    public KeyValuePair<string, string> DataObjectTocTemplate { get; private set; }
    public KeyValuePair<string, string> CodeSystemTocTemplate { get; private set; }
    public KeyValuePair<string, string> ExtensionTocTemplate { get; private set; }
    public KeyValuePair<string, string> CapabilityStatementTocTemplate { get; private set; }

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
        LoadCapabilityStatementTemplate();
        LoadCopyPasteFiles();
        LoadTerminologyFixFiles();
        LoadTocTemplates();
    }

    private void LoadTocTemplates()
    {
        DirectoryInfo dir = new(DataObjectFolderPath);
        FileInfo doTocTemplate = dir.EnumerateFiles("*.yaml", SearchOption.TopDirectoryOnly).First();
        DataObjectTocTemplate = new KeyValuePair<string, string>("toc.yaml", File.ReadAllText(doTocTemplate.FullName));

        DirectoryInfo dirCs = new(CodeSystemTemplateFolder);
        FileInfo csTocTemplate = dirCs.EnumerateFiles("*.yaml", SearchOption.TopDirectoryOnly).First();
        CodeSystemTocTemplate = new KeyValuePair<string, string>("toc.yaml", File.ReadAllText(csTocTemplate.FullName));
        
        DirectoryInfo dirEx = new(ExtensionTemplateFolder);
        FileInfo exTocTemplate = dirEx.EnumerateFiles("*.yaml", SearchOption.TopDirectoryOnly).First();
        ExtensionTocTemplate = new KeyValuePair<string, string>("toc.yaml", File.ReadAllText(exTocTemplate.FullName));
        
        DirectoryInfo dirCapStmt = new(CapabilityStatementFolder);
        FileInfo capStmtTocTemplate = dirCapStmt.EnumerateFiles("*.yaml", SearchOption.TopDirectoryOnly).First();
        CapabilityStatementTocTemplate = new KeyValuePair<string, string>("toc.yaml", File.ReadAllText(capStmtTocTemplate.FullName));
    }

    private void LoadCopyPasteFiles()
    {
        DirectoryInfo dir = new(DataObjectFolderPath);
        foreach (FileInfo fileInfo in dir.EnumerateFiles("*.md", SearchOption.AllDirectories))
        {
            if (!fileInfo.Name.Contains("$$"))
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
        FileInfo? file = dir.EnumerateFiles().FirstOrDefault(e => e.Name.StartsWith("$$"));
        _extensionTemplate = new KeyValuePair<string, string>(file!.Name, File.ReadAllText(file.FullName));
    }
    
    private void LoadCapabilityStatementTemplate()
    {
        DirectoryInfo dir = new(CapabilityStatementFolder);
        FileInfo? file = dir.EnumerateFiles().FirstOrDefault(e => e.Name.Contains("$$"));
        _capabilityStatementTemplate = new KeyValuePair<string, string>(file!.Name, File.ReadAllText(file.FullName));
    }

    public IDictionary<string, string> ApplyProfileVariables(IVariable variables) =>
        _dataObjectTemplates
            .ToDictionary(
                file => variables.ApplyVariables(file.Key), 
                file => file.Key.Contains("Beispiele")
                    ? HandleExample(file.Value, variables)
                    : variables.ApplyVariables(file.Value));
    
    public KeyValuePair<string, string> ApplyVariables(IVariable variables) =>
        variables switch
        {
            CodeSystemVariables => new KeyValuePair<string, string>(
                variables.ApplyVariables(_codeSystemTemplate.Key),
                variables.ApplyVariables(_codeSystemTemplate.Value)),
            ExtensionVariables => new KeyValuePair<string, string>(
                variables.ApplyVariables(_extensionTemplate.Key),
                variables.ApplyVariables(_extensionTemplate.Value)),
            CapabilityStatementVariables => new KeyValuePair<string, string>(
                variables.ApplyVariables(_capabilityStatementTemplate.Key),
                variables.ApplyVariables(_capabilityStatementTemplate.Value)),
            _ => throw new ArgumentOutOfRangeException(nameof(variables), variables, null)
        };

    private string HandleExample(string content, IVariable variables)
    {
/*        Match match = ExampleRegex().Match(content);
        string resultContent = variables.ApplyVariables(content.Replace("\n" + match.Value, ""));

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
            .Replace(IVariable.ENDEXAMPLE,"");*/
        return string.Empty;
    }
    
    public string ApplyTocList(string content, IEnumerable<IVariable> variables)
    {
        Match match = TocObjectRegex().Match(content);
        IEnumerable<string> allTocObjects = variables.Select(e=>e.ApplyVariables(match.Value));

        string result = allTocObjects.Aggregate(content, (current, exampleContent) => current + exampleContent);
        
        return result
            .Replace(match.Value, "")
            .ReplaceVars();
    }

    [GeneratedRegex(@"%igg\.startExample\s*(.*?)\s*%igg\.endExample", RegexOptions.Singleline)]
    private static partial Regex ExampleRegex();
    
    [GeneratedRegex(@"\$\$startTocObject\s*(.*?)\s*\$\$endTocObject", RegexOptions.Singleline)]
    private static partial Regex TocObjectRegex();
}