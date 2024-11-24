using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Source;

namespace IgGenerator.ResourceHandling.Interfaces;

public interface IResourceFileHandler
{
    public CapabilityStatement? CapabilityStatement { get; }
    public IEnumerable<FileInfo>? AllJsonFiles { get; }
    public void StartConsoleWorkflow();
    public CachedResolver GetCachedResolver();
}