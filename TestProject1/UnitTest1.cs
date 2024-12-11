using FluentAssertions;
using Hl7.Fhir.Model;
using IgGenerator.ConsoleHandling.Interfaces;
using IgGenerator.DataObjectHandling;
using IgGenerator.DataObjectHandling.Interfaces;
using IgGenerator.Helpers;
using IgGenerator.IgHandling;
using IgGenerator.IgHandling.Interfaces;
using IgGenerator.ResourceHandling;
using IgGenerator.ResourceHandling.Interfaces;
using IgGenerator.Simplifier;
using Moq;

namespace TestProject1;

public class UnitTest1
{
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
    public void Extensions_FindFolderPath()
    {
        DirectoryInfo directoryInfo = new("./IgTemplateCheck");

        string? path = directoryInfo.FindFolderPath("Datenobjekte");

        path.Should().EndWith(@"IgTemplateCheck\Einfuehrung\Datenobjekte");
    }

    [Fact]
    public void SimplifierConnector_LoadProject()
    {
        //Arrange
        Mock<IUserInteractionHandler> userInteractionHandlerMock = new();
        userInteractionHandlerMock
            .Setup(x => x.GetString(It.Is<string>(s => s == "Simplifier Username:")))
            .Returns("js@gefyra.de");
        userInteractionHandlerMock
            .Setup(x => x.GetString(It.Is<string>(s => s == "Simplifier Password:")))
            .Returns(""); //TODO Löschen
        userInteractionHandlerMock
            .Setup(x => x.GetString(It.Is<string>(s => s == "Simplifier Project Url:")))
            .Returns("https://api.simplifier.net/isik-basis-v4/zip");
        ISimplifierConnector connector = new SimplifierConnector(userInteractionHandlerMock.Object);
        
        //Act
        connector.LoadTemplate();
        
        //Assert
        Directory.Exists("Project").Should().BeTrue();
    }
}