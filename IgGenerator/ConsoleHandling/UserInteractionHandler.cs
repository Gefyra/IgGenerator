using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using IgGenerator.ConsoleHandling.Interfaces;

namespace IgGenerator.ConsoleHandling;

/// <summary>
/// Handles user interaction through the console with caching support.
/// </summary>
public class UserInteractionHandler : IUserInteractionHandler
{
    private readonly IUserInputCacheService _cacheService;

    public UserInteractionHandler(IUserInputCacheService cacheService)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    public int GetNumber(string question, int defaultAnswer)
    {
        if (string.IsNullOrEmpty(question))
            throw new ArgumentNullException(nameof(question));

        if (_cacheService.TryGetCachedValue(question, out string? value))
        {
            return int.Parse(value);
        }

        Console.WriteLine(question);
        string? input = Console.ReadLine();

        int result;
        if (string.IsNullOrEmpty(input))
        {
            result = defaultAnswer;
        }
        else if (!int.TryParse(input, out result))
        {
            Console.WriteLine($"Invalid input. Using default value: {defaultAnswer}");
            result = defaultAnswer;
        }

        _cacheService.AddOrUpdateCache(question, result.ToString());
        return result;
    }

    public string GetString(string question)
    {
        if (string.IsNullOrEmpty(question))
            throw new ArgumentNullException(nameof(question));

        if (_cacheService.TryGetCachedValue(question, out string? value))
        {
            return value;
        }

        Console.WriteLine(question);
        string? input;
        do
        {
            input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Please enter a non-empty value:");
            }
        } while (string.IsNullOrEmpty(input));

        _cacheService.AddOrUpdateCache(question, input);
        return input;
    }

    public void Send(string message)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException(nameof(message));

        Console.WriteLine(message);
    }

    public void SendAndExit(string message)
    {
        Send(message);
        Environment.Exit(0);
    }

    public bool AskYesNoQuestion(string question, bool defaultAnswer)
    {
        if (string.IsNullOrEmpty(question))
            throw new ArgumentNullException(nameof(question));

        if (question != "Use Cache?" && _cacheService.TryGetCachedValue(question, out string? value))
        {
            return bool.Parse(value);
        }

        Console.WriteLine(question);
        string? answer;
        bool result;

        do
        {
            answer = Console.ReadLine()?.Trim().ToLower();
            
            result = answer switch
            {
                "y" or "yes" or "t" => true,
                "n" or "no" or "f" => false,
                "" => defaultAnswer,
                _ => !bool.TryParse(answer, out result) ? defaultAnswer : result
            };

            if (string.IsNullOrEmpty(answer))
            {
                Console.WriteLine($"Using default value: {defaultAnswer}");
            }
        } while (answer != null && !IsValidBooleanInput(answer) && !string.IsNullOrEmpty(answer));

        _cacheService.AddOrUpdateCache(question, result.ToString());
        return result;
    }

    public void AskCacheUsage()
    {
        _cacheService.ConfigureCache(AskYesNoQuestion);
    }

    private static bool IsValidBooleanInput(string input)
    {
        return input.ToLower() switch
        {
            "y" or "yes" or "t" or "true" or
            "n" or "no" or "f" or "false" => true,
            _ => bool.TryParse(input, out _)
        };
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