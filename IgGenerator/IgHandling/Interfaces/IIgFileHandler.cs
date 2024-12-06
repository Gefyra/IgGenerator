namespace IgGenerator.IgHandling.Interfaces;

public interface IIgFileHandler
{
    public void StartConsoleWorkflow();
    public void SaveExtractedDataObjectFiles(IDictionary<string, IDictionary<string, string>> extractedDataObjects);
    public void SaveExtractedCodeSystemFiles(IDictionary<string, string> extractedCodeSystems);
    public void SaveExtractedExtensionFiles(IDictionary<string, string> extractedExtensions);
    public void SaveExtractedCapStmtFiles(IDictionary<string, string> extractedCapStmt);
    public void SaveCopyPasteFiles();
    public void SaveTocFiles();
}