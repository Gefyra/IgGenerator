namespace IgGenerator.DataObjectHandling.Interfaces;

public record Template(TemplateType[] TemplateType, string FileName, string Content);

public enum TemplateType
{
    DataObject,
    CodeSystem,
    Extension,
    CapabilityStatement,
    TocFile,
    CopyPasteFile
}