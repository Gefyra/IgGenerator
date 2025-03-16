using System.Text.RegularExpressions;
using IgGenerator.DataObjectHandling.Interfaces;
using IgGenerator.Helpers;

namespace IgGenerator.DataObjectHandling.Services;

/// <summary>
/// Service responsible for loading and managing templates from the file system.
/// </summary>
public class TemplateLoadingService
{
    // Path constants
    private const string DATA_OBJECT_FOLDER_NAME = "Datenobjekte";
    private const string DATA_OBJECT_FOLDER_PATH = "./IgTemplate/Home/Datenobjekte";
    private const string SINGLE_DATA_OBJECT_TEMPLATE_FOLDER = $"{DATA_OBJECT_FOLDER_PATH}/Datenobjekte_Template";
    private const string CODE_SYSTEM_TEMPLATE_FOLDER = $"{DATA_OBJECT_FOLDER_PATH}/Terminologien/";
    private const string EXTENSION_TEMPLATE_FOLDER = $"{DATA_OBJECT_FOLDER_PATH}/Extensions/";
    private const string CAPABILITY_STATEMENT_FOLDER = $"{DATA_OBJECT_FOLDER_PATH}/CapabilityStatements/";

    // File pattern constants
    private const string YAML_PATTERN = "*.yaml";
    private const string MARKDOWN_PATTERN = "*.md";
    private const string TEMPLATE_MARKER = "$$";
    private const string TOC_FILENAME = "toc.yaml";

    /// <summary>
    /// Loads all templates from the file system.
    /// </summary>
    /// <returns>A collection of loaded templates</returns>
    /// <exception cref="InvalidOperationException">Thrown when templates cannot be loaded</exception>
    public IEnumerable<Template> LoadAllTemplates()
    {
        var templates = new List<Template>();
        
        try
        {
            Console.WriteLine("Starting template loading process...");
            
            templates.AddRange(LoadSingleDataObjectTemplates());
            Console.WriteLine($"Loaded {templates.Count} data object templates");
            
            templates.Add(LoadSingleTemplateFile(TemplateType.CodeSystem, CODE_SYSTEM_TEMPLATE_FOLDER));
            templates.Add(LoadSingleTemplateFile(TemplateType.Extension, EXTENSION_TEMPLATE_FOLDER));
            templates.Add(LoadSingleTemplateFile(TemplateType.CapabilityStatement, CAPABILITY_STATEMENT_FOLDER));
            Console.WriteLine("Loaded single template files for CodeSystem, Extension, and CapabilityStatement");
            
            var copyPasteFiles = LoadCopyPasteFiles().ToList();
            templates.AddRange(copyPasteFiles);
            Console.WriteLine($"Loaded {copyPasteFiles.Count} copy-paste files");
            
            var tocTemplates = LoadTocTemplates().ToList();
            templates.AddRange(tocTemplates);
            Console.WriteLine($"Loaded {tocTemplates.Count} TOC templates");
            
            if (!templates.Any())
            {
                throw new InvalidOperationException("No templates were loaded from the file system");
            }
            
            Console.WriteLine($"Successfully loaded {templates.Count} templates in total");
            return templates;
        }
        catch (Exception ex) when (ex is DirectoryNotFoundException or FileNotFoundException)
        {
            var error = $"Required template files or directories are missing: {ex.Message}";
            Console.WriteLine(error);
            throw new InvalidOperationException(error, ex);
        }
        catch (Exception ex)
        {
            var error = $"Unexpected error while loading templates: {ex.Message}";
            Console.WriteLine(error);
            throw new InvalidOperationException(error, ex);
        }
    }

    private IEnumerable<Template> LoadTocTemplates()
    {
        Console.WriteLine("Loading TOC templates...");
        var templateMappings = new[]
        {
            (Path: DATA_OBJECT_FOLDER_PATH, Type: TemplateType.TocFile | TemplateType.DataObject),
            (Path: CODE_SYSTEM_TEMPLATE_FOLDER, Type: TemplateType.TocFile | TemplateType.CodeSystem),
            (Path: EXTENSION_TEMPLATE_FOLDER, Type: TemplateType.TocFile | TemplateType.Extension),
            (Path: CAPABILITY_STATEMENT_FOLDER, Type: TemplateType.TocFile | TemplateType.CapabilityStatement)
        };

        return templateMappings.Select(mapping => LoadTocTemplate(mapping.Path, mapping.Type));
    }

    private Template LoadTocTemplate(string path, TemplateType templateType)
    {
        DirectoryInfo directory = new DirectoryInfo(path);
        FileInfo tocTemplate = directory.EnumerateFiles(YAML_PATTERN, SearchOption.TopDirectoryOnly).First();
        string content = File.ReadAllText(tocTemplate.FullName);
        
        var template = new Template(templateType, TOC_FILENAME, content);
        Console.WriteLine($"Loaded TOC template for {templateType}");
        return template;
    }

    private IEnumerable<Template> LoadCopyPasteFiles()
    {
        Console.WriteLine("Loading copy-paste files...");
        DirectoryInfo directory = new DirectoryInfo(DATA_OBJECT_FOLDER_PATH);
        IEnumerable<FileInfo> markdownFiles = directory.EnumerateFiles(MARKDOWN_PATTERN, SearchOption.AllDirectories)
            .Where(file => !file.Name.Contains(TEMPLATE_MARKER));

        return markdownFiles.Select(fileInfo =>
        {
            string relativePath = fileInfo.FullName.GetPathFromFolder(DATA_OBJECT_FOLDER_NAME)!;
            string content = File.ReadAllText(fileInfo.FullName);
            
            var template = new Template(TemplateType.CopyPasteFile, relativePath, content);
            Console.WriteLine($"Loaded copy-paste file: {relativePath}");
            return template;
        });
    }

    private IEnumerable<Template> LoadSingleDataObjectTemplates()
    {
        Console.WriteLine("Loading data object templates...");
        DirectoryInfo directory = new DirectoryInfo(SINGLE_DATA_OBJECT_TEMPLATE_FOLDER);
        
        return directory.EnumerateFiles().Select(file =>
        {
            string content = File.ReadAllText(file.FullName);
            var template = new Template(TemplateType.DataObject, file.Name, content);
            Console.WriteLine($"Loaded data object template: {file.Name}");
            return template;
        });
    }

    private Template LoadSingleTemplateFile(TemplateType templateType, string folderPath)
    {
        Console.WriteLine($"Loading template for {templateType}...");
        DirectoryInfo directory = new DirectoryInfo(folderPath);
        FileInfo? templateFile = directory.EnumerateFiles()
            .FirstOrDefault(file => file.Name.Contains(TEMPLATE_MARKER));

        if (templateFile == null)
        {
            throw new FileNotFoundException($"No template file found for {templateType} in {folderPath}");
        }

        string content = File.ReadAllText(templateFile.FullName);
        var template = new Template(templateType, templateFile.Name, content);
        Console.WriteLine($"Loaded template: {templateFile.Name}");
        return template;
    }
} 