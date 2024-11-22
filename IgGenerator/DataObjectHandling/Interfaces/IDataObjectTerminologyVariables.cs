namespace IgGenerator.DataObjectHandling.Interfaces;

public interface IDataObjectTerminologyVariables : IVariable
{
    public string? TerminologyName { get; set; }
    public string? Canonical { get; set; }
}