using IgGenerator.ConsoleHandling;
using IgGenerator.IgHandling;
using IgGenerator.ResourceHandling;

namespace IgGenerator;

public class Application(
    IIgHandler igHandler,
    IIgFileHandler igFileHandler,
    IResourceFileHandler resourceFileHandler,
    INamingManipulationHandler namingManipulationHandler,
    IUserInteractionHandler userInteractionHandler)
    : IApplication
{
    private readonly INamingManipulationHandler _namingManipulationHandler = namingManipulationHandler;

    public void StartWorkflow()
    {
        userInteractionHandler.AskCacheUsage();
        
        resourceFileHandler.StartConsoleWorkflow();
        IDictionary<string, IDictionary<string, string>> appliedDataObjects = igHandler.ApplyTemplateToAllSupportedProfiles();
        IDictionary<string, string> appliedCodeSystems = igHandler.ApplyTemplateToCodeSystems();
        IDictionary<string, string> appliedExtensions = igHandler.ApplyTemplateToExtensions();

        namingManipulationHandler.StartConsoleWorkflow();

        igFileHandler.StartConsoleWorkflow();
        igFileHandler.SaveExtractedDataObjectFiles(appliedDataObjects);
        igFileHandler.SaveExtractedCodeSystemFiles(appliedCodeSystems);
        igFileHandler.SaveExtractedExtensionFiles(appliedExtensions);
        igFileHandler.SaveCopyPasteFiles();
        igFileHandler.SaveTocFiles();
    }
}