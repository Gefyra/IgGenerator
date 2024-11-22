using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using IgGenerator.ConsoleHandling.Interfaces;

namespace IgGenerator.ConsoleHandling;

public class UserInteractionHandler : IUserInteractionHandler
{
    private DictionaryXmlable<string, string> _cache;
    private bool _useCache = true;


    public UserInteractionHandler()
    {
        RestoreCache();
    }

    public int GetNumber(string question, int defaultAnswer)
    {
        if (_useCache && _cache.TryGetValue(question, out string? value)) return int.Parse(value);
        Console.WriteLine(question);
        int number = int.Parse(Console.ReadLine() ?? $"{defaultAnswer}");
        AddToCache(question, number.ToString());
        SaveCache();
        return number;
    }

    public string GetString(string question)
    {
        if (_useCache && _cache.TryGetValue(question, out string? value)) return value;
        Console.WriteLine(question);
        string? stringValue;
        do
        {
            stringValue = Console.ReadLine();
        } while (string.IsNullOrEmpty(stringValue));

        AddToCache(question, stringValue);
        SaveCache();
        return stringValue;
    }

    public void Send(string message)
    {
        Console.WriteLine(message);
    }

    public void SendAndExit(string message)
    {
        Send(message);
        Environment.Exit(0);
    }

    public bool AskYesNoQuestion(string question, bool defaultAnswer)
    {
        if (question != "Use Cache?" && _useCache && _cache.TryGetValue(question, out string? value))
            return bool.Parse(value);
        Console.WriteLine(question);
        string? answer;
        do
        {
            answer = Console.ReadLine();
            if (answer?.ToLower() == "y" || answer?.ToLower() == "t")
            {
                answer = "true";
            }

            if (answer?.ToLower() == "n" || answer?.ToLower() == "f")
            {
                answer = "false";
            }
        } while (string.IsNullOrEmpty(answer) && bool.TryParse(answer, out _));

        AddToCache(question, answer!);
        SaveCache();
        return bool.Parse(answer!);
    }

    private void AddToCache(string question, string answer)
    {
        if (_cache.ContainsKey(question))
        {
            _cache[question] = answer;
        }
        else
        {
            _cache.Add(question, answer!);
        }
    }

    public void AskCacheUsage()
    {
        _useCache = AskYesNoQuestion("Use Cache?", true);
    }

    public void SaveCache()
    {
        XmlSerializer serializer = new(typeof(DictionaryXmlable<string, string>));
        TextWriter textWriter = new StreamWriter(@"./cache.xml");
        serializer.Serialize(textWriter, _cache);
        textWriter.Close();
    }

    private void RestoreCache()
    {
        if (File.Exists("./cache.xml"))
        {
            XmlSerializer serializer = new(typeof(DictionaryXmlable<string, string>));
            TextReader textReader = new StreamReader(@"./cache.xml");
            _cache = (DictionaryXmlable<string, string>)serializer.Deserialize(textReader)!;
            textReader.Close();
        }
        else
        {
            _cache = new DictionaryXmlable<string, string>();
        }
    }
}

[XmlRoot("Languages")]
public class DictionaryXmlable<TKey, TValue> : Dictionary<TKey, TValue>,
    IXmlSerializable where TKey : notnull
{
    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        if (reader.IsEmptyElement)
        {
            return;
        }

        reader.Read();
        while (reader.NodeType != XmlNodeType.EndElement)
        {
            object key = reader.GetAttribute("Title");
            object value = reader.GetAttribute("Value");
            this.Add((TKey)key, (TValue)value);
            reader.Read();
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        foreach (var key in this.Keys)
        {
            writer.WriteStartElement("Language");
            writer.WriteAttributeString("Title", key.ToString());
            writer.WriteAttributeString("Value", this[key].ToString());
            writer.WriteEndElement();
        }
    }
}