﻿using IgGenerator.IgHandling;
using IgGenerator.ResourceHandling;

namespace IgGenerator;

public class Application(
    IIgHandler igHandler,
    IIgFileHandler igFileHandler,
    IResourceFileHandler resourceFileHandler,
    INamingManipulationHandler namingManipulationHandler)
    : IApplication
{
    private readonly INamingManipulationHandler _namingManipulationHandler = namingManipulationHandler;

    public void StartWorkflow()
    {
        resourceFileHandler.StartConsoleWorkflow();
        IDictionary<string, IDictionary<string, string>> appliedDataObjects = igHandler.ApplyTemplateToAllSupportedProfiles();
        
        namingManipulationHandler.StartConsoleWorkflow();
        
        igFileHandler.StartConsoleWorkflow();
        igFileHandler.SaveExtractedDataObjectFiles(appliedDataObjects);
    }
}