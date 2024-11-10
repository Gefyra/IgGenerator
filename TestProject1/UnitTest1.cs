using System.Collections;
using FluentAssertions;
using IgGenerator;
using IgGenerator.DataObjectHandling;

namespace TestProject1;

public class UnitTest1
{
    
    [Fact]
    public void DataObjectReplacementTest()
    {
        DataObjectTemplateHandler objectTemplateHandler = new();
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
        IDictionary<string, string> files = objectTemplateHandler.ApplyVariables(variables);

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
}