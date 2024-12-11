using IgGenerator.ConsoleHandling.Interfaces;
using IgGenerator.IgHandling;
using IgGenerator.IgHandling.Interfaces;
using IgGenerator.ResourceHandling.Interfaces;
using IgGenerator.Simplifier;

namespace IgGenerator;

public class Application(
    IIgHandler igHandler,
    IIgFileHandler igFileHandler,
    IResourceFileHandler resourceFileHandler,
    INamingManipulationHandler namingManipulationHandler,
    IUserInteractionHandler userInteractionHandler,
    ISimplifierConnector simplifierConnector)
    : IApplication
{
    public void StartWorkflow()
    {
        userInteractionHandler.AskCacheUsage(); 
        
        namingManipulationHandler.StartConsoleWorkflow();

        simplifierConnector.LoadTemplate();
        
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