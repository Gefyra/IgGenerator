namespace IgGenerator.DataObjectHandling.Interfaces;

public record Template(TemplateType TemplateType, string FileName, string Content);

[Flags]
public enum TemplateType
{
    DataObject = 0,
    CodeSystem = 1,
    Extension = 2,
    CapabilityStatement = 4,
    TocFile = 8,
    CopyPasteFile = 16
}