using Hl7.Fhir.Model;

namespace IgGenerator.ResourceHandling.Interfaces;

public interface IResourceHandler
{
    public IEnumerable<string>? ExtractSupportedProfiles();
    public StructureDefinition? GetStructureDefinition(string supportedProfile);
    public IEnumerable<CodeSystem> GetCodeSystems();
    public IEnumerable<CapabilityStatement> GetCapabilityStatements();
    public IEnumerable<Resource> GetExamplesForProfile(string supportedProfile);
    public IEnumerable<(string name, string canonical)> GetUsedExtensions();
}