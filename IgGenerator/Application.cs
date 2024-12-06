using IgGenerator.ConsoleHandling;
using IgGenerator.ConsoleHandling.Interfaces;
using IgGenerator.IgHandling;
using IgGenerator.IgHandling.Interfaces;
using IgGenerator.ResourceHandling;
using IgGenerator.ResourceHandling.Interfaces;

namespace IgGenerator;

public class Application(
    IIgHandler igHandler,
    IIgFileHandler igFileHandler,
    IResourceFileHandler resourceFileHandler,
    INamingManipulationHandler namingManipulationHandler,
    IUserInteractionHandler userInteractionHandler)
    : IApplication
{
    public void StartWorkflow()
    {
        userInteractionHandler.AskCacheUsage(); 
        
        namingManipulationHandler.StartConsoleWorkflow();

        resourceFileHandler.StartConsoleWorkflow();
        IDictionary<string, IDictionary<string, string>> appliedDataObjects = igHandler.ApplyTemplateToAllSupportedProfiles();
        IDictionary<string, string> appliedCodeSystems = igHandler.ApplyTemplateToCodeSystems();
        IDictionary<string, string> appliedExtensions = igHandler.ApplyTemplateToExtensions();
        IDictionary<string, string> appliedCapStmt = igHandler.ApplyTemplateToCapabilityStatement();


        igFileHandler.StartConsoleWorkflow();
        igFileHandler.SaveExtractedDataObjectFiles(appliedDataObjects);
        igFileHandler.SaveExtractedCodeSystemFiles(appliedCodeSystems);
        igFileHandler.SaveExtractedExtensionFiles(appliedExtensions);
        igFileHandler.SaveExtractedCapStmtFiles(appliedCapStmt);
        igFileHandler.SaveCopyPasteFiles();
        igFileHandler.SaveTocFiles();
    }
}