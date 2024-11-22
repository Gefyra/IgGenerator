using FluentAssertions;
using Hl7.Fhir.Model;
using IgGenerator.ConsoleHandling;
using IgGenerator.ConsoleHandling.Interfaces;
using IgGenerator.DataObjectHandling;
using IgGenerator.DataObjectHandling.Interfaces;
using IgGenerator.Helpers;
using IgGenerator.IgHandling;
using IgGenerator.IgHandling.Interfaces;
using IgGenerator.ResourceHandling;
using IgGenerator.ResourceHandling.Interfaces;
using Moq;

namespace TestProject1;

public class UnitTest1
{
    
    [Fact]
    public void DataObjectTemplateHandler_DataObjectReplacementTest()
    {
        TemplateHandler templateHandler = new(new NamingManipulationHandler(null!));
        IDictionary<string, string> example = new Dictionary<string, string>()
        {
            { "Valides Minimalbeispiel Patient", "patientinmusterfrau" },
        };
        IDataObjectVariables variables = CreateDataObjectVariables
            .WithResourceName("Patient")
            .WithCanonical("https://gematik.de/fhir/isik/StructureDefinition/ISiKPatient")
            .WithCoreUrl("https://hl7.org/fhir/R4/patient.html")
            .WithExampleNamesAndIds(example);
        string checkFolderName = "./IgTemplateCheck/Einfuehrung/Datenobjekte/Datenobjekte_Template";
        DirectoryInfo directoryInfo = new(checkFolderName);
        IDictionary<string, string> _check = new Dictionary<string, string>();
        foreach (FileInfo enumerateFile in directoryInfo.EnumerateFiles())
        {
            _check.Add(enumerateFile.Name, File.ReadAllText(enumerateFile.FullName));
        }

        //Act
        IDictionary<string, string> files = templateHandler.ApplyProfileVariables(variables);

        //Assert
        foreach (KeyValuePair<string, string> pair in _check)
        {
            if (pair.Key.Contains("toc"))
            {
                continue;
            }
            files.Should().ContainKey(pair.Key);
            files[pair.Key].Should().BeEquivalentTo(pair.Value);
        }
    }

    [Fact]
    public void ResourceFileHandler_FindCapabilityStatementTest()
    {
        Mock<IUserInteractionHandler> userInterationHandlerMock = new();
        userInterationHandlerMock
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns("./ResourcesCheck/Resources");
        userInterationHandlerMock.Setup(x => x.GetNumber(It.IsAny<string>(), It.IsAny<int>())).Returns(0);
        
        IResourceFileHandler fileHandler = new ResourceFileHandler(userInterationHandlerMock.Object);

        //Act
        fileHandler.StartConsoleWorkflow();
        
        //Assert
        fileHandler.CapabilityStatement.Should().NotBeNull();
        fileHandler.CapabilityStatement.Id.Should().BeEquivalentTo("ISiKCapabilityStatementBasisServer");
    }
    
    [Fact]
    public void ResourceHandler_GetStructureDefinitionTest()
    {
        //Arrange
        Mock<IUserInteractionHandler> userInterationHandlerMock = new();
        userInterationHandlerMock
            .Setup(x => x.GetString(It.Is<string>(s => s == "Path of Resources:")))
            .Returns("./ResourcesCheck/Resources");
        userInterationHandlerMock.Setup(x => x.GetNumber(It.IsAny<string>(), It.IsAny<int>())).Returns(0);
        string canonical = "https://gematik.de/fhir/isik/StructureDefinition/ISiKPatient";
        
        IResourceFileHandler fileHandler = new ResourceFileHandler(userInterationHandlerMock.Object);

        IResourceHandler resourceHandler = new ResourceHandler(fileHandler, userInterationHandlerMock.Object);

        //Act
        fileHandler.StartConsoleWorkflow();
        StructureDefinition supportedProfile = resourceHandler.GetStructureDefinition(canonical);
        
        //Assert
        supportedProfile.Should().NotBeNull();
        supportedProfile.Id.Should().Be("ISiKPatient");
    }
    
    [Fact]
    public void ResourceHandler_ExtractSupportedProfilesTest()
    {
        Mock<IUserInteractionHandler> userInterationHandlerMock = new();
        userInterationHandlerMock
            .Setup(x => x.GetString(It.Is<string>(s => s == "Path of Resources:")))
            .Returns("./ResourcesCheck/Resources");
        userInterationHandlerMock.Setup(x => x.GetNumber(It.IsAny<string>(), It.IsAny<int>())).Returns(0);
        
        IResourceFileHandler fileHandler = new ResourceFileHandler(userInterationHandlerMock.Object);

        IResourceHandler resourceHandler = new ResourceHandler(fileHandler, userInterationHandlerMock.Object);

        //Act
        fileHandler.StartConsoleWorkflow();
        IEnumerable<string>? supportedProfiles = resourceHandler.ExtractSupportedProfiles()?.ToArray();
        
        //Assert
        supportedProfiles.Should().NotBeNull();
        supportedProfiles.Count().Should().Be(26);
    }
    
    [Fact]
    public void ResourceHandler_GetCodeSystems()
    {
        Mock<IUserInteractionHandler> userInterationHandlerMock = new();
        userInterationHandlerMock
            .Setup(x => x.GetString(It.Is<string>(s => s == "Path of Resources:")))
            .Returns("./ResourcesCheck/Resources");
        userInterationHandlerMock.Setup(x => x.GetNumber(It.IsAny<string>(), It.IsAny<int>())).Returns(0);
        
        IResourceFileHandler fileHandler = new ResourceFileHandler(userInterationHandlerMock.Object);

        IResourceHandler resourceHandler = new ResourceHandler(fileHandler, userInterationHandlerMock.Object);

        //Act
        fileHandler.StartConsoleWorkflow();
        IEnumerable<CodeSystem> codeSystems = resourceHandler.GetCodeSystems().ToArray();
        
        //Assert
        codeSystems.Should().NotBeNull();
        codeSystems.Count().Should().Be(5);
    }
    
    [Fact]
    public void ResourceHandler_GetUsedExtensions()
    {
        Mock<IUserInteractionHandler> userInterationHandlerMock = new();
        userInterationHandlerMock
            .Setup(x => x.GetString(It.Is<string>(s => s == "Path of Resources:")))
            .Returns("./ResourcesCheck/Resources");
        userInterationHandlerMock.Setup(x => x.GetNumber(It.IsAny<string>(), It.IsAny<int>())).Returns(0);
        
        IResourceFileHandler fileHandler = new ResourceFileHandler(userInterationHandlerMock.Object);

        IResourceHandler resourceHandler = new ResourceHandler(fileHandler, userInterationHandlerMock.Object);

        //Act
        fileHandler.StartConsoleWorkflow();
        (string name, string canonical)[] extensions = resourceHandler.GetUsedExtensions().ToArray();
        
        //Assert
        extensions.Should().NotBeNull();
        extensions.Count().Should().Be(13);
    }
    
    [Fact]
    public void DataObjectTemplateHandler_LoadCopyPasteFiles()
    {
        //Act
        ITemplateHandler templateHandler = new TemplateHandler(null!);

        //Assert
        templateHandler.CopyPasteFiles.Count.Should().Be(9);
    }
    
    [Fact]
    public void TocFileManager_GetDataObjectTocFile()
    {
        //Arrange

        Mock<IUserInteractionHandler> userInterationHandlerMock = new();
        userInterationHandlerMock
            .Setup(x => x.GetString(It.Is<string>(s => s == "Path of Resources:")))
            .Returns("./ResourcesCheck/Resources");
        userInterationHandlerMock
            .Setup(x=>x.GetString(It.Is<string>(s => s == "Do you want to manipulate names? (default: false)")))
            .Returns("y");
        userInterationHandlerMock.Setup(x => x.GetNumber(It.IsAny<string>(), It.IsAny<int>())).Returns(0);
        ITemplateHandler templateHandler = new TemplateHandler(new NamingManipulationHandler(userInterationHandlerMock.Object));
        ITocFileManager tocFileManager = new TocFileManager(templateHandler);
        IResourceFileHandler fileHandler = new ResourceFileHandler(userInterationHandlerMock.Object);
        IResourceHandler resourceHandler = new ResourceHandler(fileHandler, userInterationHandlerMock.Object);
        IIgHandler igHandler = new IgHandler(resourceHandler, templateHandler, fileHandler, null!, tocFileManager);

        //Act
        igHandler.ApplyTemplateToAllSupportedProfiles();
        string tocFile = tocFileManager.GetDataObjectTocFile();

        //Assert
        tocFile.Should().NotBeNull();
    }
    
    [Fact]
    public void IgHanlder_ApplyTemplateToAllSupportedProfilesTest()
    {
        Mock<IUserInteractionHandler> userInterationHandlerMock = new();
        userInterationHandlerMock
            .Setup(x => x.GetString(It.Is<string>(s => s == "Path of Resources:")))
            .Returns("./ResourcesCheck/Resources");
        userInterationHandlerMock
            .Setup(x=>x.GetString(It.Is<string>(s => s == "Do you want to manipulate names? (default: false)")))
            .Returns("y");
        userInterationHandlerMock.Setup(x => x.GetNumber(It.IsAny<string>(), It.IsAny<int>())).Returns(0);
        IResourceFileHandler fileHandler = new ResourceFileHandler(userInterationHandlerMock.Object);
        IResourceHandler resourceHandler = new ResourceHandler(fileHandler, userInterationHandlerMock.Object);
        IIgHandler igHandler = new IgHandler(resourceHandler, new TemplateHandler(new NamingManipulationHandler(userInterationHandlerMock.Object)), fileHandler, null!, null!);
        
        string checkFolderName = "./IgTemplateCheck/Einfuehrung/Datenobjekte";
        DirectoryInfo directoryInfo = new(checkFolderName);
        IDictionary<string, string> _check = new Dictionary<string, string>();
        foreach (FileInfo enumerateFile in directoryInfo.EnumerateFiles())
        {
            _check.Add(enumerateFile.Name, File.ReadAllText(enumerateFile.FullName));
        }
        
        //Act
        IDictionary<string, IDictionary<string, string>> filledTemplates = igHandler.ApplyTemplateToAllSupportedProfiles();
        
        //Assert
    }

    [Fact]
    public void Extensions_FindFolderPath()
    {
        DirectoryInfo directoryInfo = new("./IgTemplateCheck");

        string? path = directoryInfo.FindFolderPath("Datenobjekte");

        path.Should().EndWith(@"IgTemplateCheck\Einfuehrung\Datenobjekte");
    }
    
}