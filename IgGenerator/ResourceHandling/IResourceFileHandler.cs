using Hl7.Fhir.Model;

namespace IgGenerator.ResourceHandling;

public interface IResourceFileHandler
{
    public CapabilityStatement? CapabilityStatement { get; }
    public IEnumerable<FileInfo>? AllJsonFiles { get; }
    public void StartWorkflow();
}