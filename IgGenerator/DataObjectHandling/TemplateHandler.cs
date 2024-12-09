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

    private List<Template> _templates = [];
    
    private readonly INamingManipulationHandler _namingManipulationHandler;
    
    public TemplateHandler(INamingManipulationHandler namingManipulationHandler)
    {
        _namingManipulationHandler = namingManipulationHandler;

        LoadSingleDataObjectTemplates();
        LoadCodeSystemTemplate();
        LoadExtensionTemplate();
        LoadCapabilityStatementTemplate();
        LoadCopyPasteFiles();
        LoadTocTemplates();
    }

    private void LoadTocTemplates()
    {
        DirectoryInfo dir = new(DataObjectFolderPath);
        FileInfo doTocTemplate = dir.EnumerateFiles("*.yaml", SearchOption.TopDirectoryOnly).First();
        _templates.Add(new Template([TemplateType.TocFile, TemplateType.DataObject], "toc.yaml", File.ReadAllText(doTocTemplate.FullName)));

        DirectoryInfo dirCs = new(CodeSystemTemplateFolder);
        FileInfo csTocTemplate = dirCs.EnumerateFiles("*.yaml", SearchOption.TopDirectoryOnly).First();
        _templates.Add(new Template([TemplateType.TocFile, TemplateType.CodeSystem], "toc.yaml", File.ReadAllText(csTocTemplate.FullName)));
        
        DirectoryInfo dirEx = new(ExtensionTemplateFolder);
        FileInfo exTocTemplate = dirEx.EnumerateFiles("*.yaml", SearchOption.TopDirectoryOnly).First();
        _templates.Add(new Template([TemplateType.TocFile, TemplateType.Extension], "toc.yaml", File.ReadAllText(exTocTemplate.FullName)));
        
        DirectoryInfo dirCapStmt = new(CapabilityStatementFolder);
        FileInfo capStmtTocTemplate = dirCapStmt.EnumerateFiles("*.yaml", SearchOption.TopDirectoryOnly).First();
        _templates.Add(new Template([TemplateType.TocFile, TemplateType.CapabilityStatement], "toc.yaml", File.ReadAllText(capStmtTocTemplate.FullName)));
    }

    private void LoadCopyPasteFiles()
    {
        DirectoryInfo dir = new(DataObjectFolderPath);
        foreach (FileInfo fileInfo in dir.EnumerateFiles("*.md", SearchOption.AllDirectories))
        {
            if (!fileInfo.Name.Contains("$$"))
            {
                _templates.Add(new Template([TemplateType.CopyPasteFile], fileInfo.FullName.GetPathFromFolder(DataObjectFolderName)!,File.ReadAllText(fileInfo.FullName)));
            }
        }
    }

    private void LoadSingleDataObjectTemplates()
    {
        DirectoryInfo dir = new(SingleDataObjectTemplateFolderName);
        foreach (FileInfo enumerateFile in dir.EnumerateFiles())
        {
            _templates.Add(new Template([TemplateType.DataObject], enumerateFile.Name,File.ReadAllText(enumerateFile.FullName)));
        }
    }

    private void LoadCodeSystemTemplate()
    {
        LoadAndAddSingleTemplateFile([TemplateType.CodeSystem], CodeSystemTemplateFolder);
    }
    
    private void LoadExtensionTemplate()
    {
        LoadAndAddSingleTemplateFile([TemplateType.Extension], ExtensionTemplateFolder);
    }

    private void LoadCapabilityStatementTemplate()
    {
        LoadAndAddSingleTemplateFile([TemplateType.CapabilityStatement], CapabilityStatementFolder);
    }
    
    private void LoadAndAddSingleTemplateFile(TemplateType[] templateTypes, string folderName)
    {
        DirectoryInfo dir = new(folderName);
        FileInfo? file = dir.EnumerateFiles().FirstOrDefault(e => e.Name.Contains("$$"));
        _templates.Add(new Template(templateTypes, file!.Name,File.ReadAllText(file.FullName)));
    }

    public KeyValuePair<string, string> ApplyVariables(IVariable variables) =>
        variables switch
        {
            CodeSystemVariables => ApplyVariablesToKeyValuePair(variables, _templates.First(e=>e.TemplateType.Contains(TemplateType.CodeSystem))),
            ExtensionVariables => ApplyVariablesToKeyValuePair(variables, _templates.First(e=>e.TemplateType.Contains(TemplateType.Extension))),
            CapabilityStatementVariables => ApplyVariablesToKeyValuePair(variables, _templates.First(e=>e.TemplateType.Contains(TemplateType.CapabilityStatement))),
            _ => throw new ArgumentOutOfRangeException(nameof(variables), variables, null)
        };

    private static KeyValuePair<string, string> ApplyVariablesToKeyValuePair(IVariable variables, Template template) =>
        new(
            variables.ApplyVariables(template.FileName),
            variables.ApplyVariables(template.Content));

    public IDictionary<string, string> ApplyProfileVariables(IVariable variables) =>
        _templates
            .Where(e=>e.TemplateType == (TemplateType[])[TemplateType.DataObject])
            .ToDictionary(
                file => variables.ApplyVariables(file.FileName), 
                file => variables.ApplyVariables(file.Content));

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

    public IEnumerable<Template> GetTemplate(TemplateType[] templateTypes)
    {
        return _templates.Where(e => e.TemplateType.Contains(templateTypes[0]) && (templateTypes.Length == 1 || e.TemplateType.Contains(templateTypes[1])));
    }

    [GeneratedRegex(@"%igg\.startExample\s*(.*?)\s*%igg\.endExample", RegexOptions.Singleline)]
    private static partial Regex ExampleRegex();
    
    [GeneratedRegex(@"\$\$startTocObject\s*(.*?)\s*\$\$endTocObject", RegexOptions.Singleline)]
    private static partial Regex TocObjectRegex();
}