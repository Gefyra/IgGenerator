namespace IgGenerator.IgHandling;

public interface IIgFileHandler
{
    public void StartConsoleWorkflow();
    public void SaveExtractedDataObjectFiles(IDictionary<string, IDictionary<string, string>> extractedDataObjects);
}