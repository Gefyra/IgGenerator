using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace IgGenerator.CapabilityStatementHandling;

public class CapabilityStatementHandler : ICapabilityStatementHandler
{
    private string? _csFilePath;
    private readonly FhirJsonParser _parser = new();
    public CapabilityStatement? CapabilityStatement { get; private set; }

    public void ReadCapabilityStatement()
    {
        CapabilityStatement = _parser.Parse<CapabilityStatement>(_csFilePath);
    }

    public void AskForInput()
    {
        Console.WriteLine("Path to CapabilityStatement:");
        _csFilePath = Console.ReadLine();
        ValidInput();
    }

    private void ValidInput()
    {
        if (_csFilePath is null)
        {
            Console.WriteLine("Input is not valid");
            AskForInput();
            return;
        }
        
        try
        {
            _ = new FileInfo(_csFilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Invalid file path: {e.Message}");
            return;
        }

        try
        {
            _parser.Parse<CapabilityStatement>(File.ReadAllText(_csFilePath));
        }
        catch (Exception e)
        {
            Console.WriteLine($"The File is no CapabilityStatement: {e.Message}");
        }
        
        Console.WriteLine("Input is valid");
    }
}

public interface ICapabilityStatementHandler
{
    public void AskForInput();
    public CapabilityStatement? CapabilityStatement { get; }
}