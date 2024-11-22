using Hl7.Fhir.Model;

namespace IgGenerator.ResourceHandling.Interfaces;

public interface IResourceFileHandler
{
    public CapabilityStatement? CapabilityStatement { get; }
    public IEnumerable<FileInfo>? AllJsonFiles { get; }
    public void StartConsoleWorkflow();
}