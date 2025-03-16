using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace IgGenerator.ConsoleHandling.Models;

/// <summary>
/// A dictionary that can be serialized to XML.
/// </summary>
/// <typeparam name="TKey">The type of the dictionary keys</typeparam>
/// <typeparam name="TValue">The type of the dictionary values</typeparam>
[XmlRoot("Cache")]
public class DictionaryXmlable<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable 
    where TKey : notnull
{
    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        
        if (reader.IsEmptyElement)
        {
            return;
        }

        reader.Read();
        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType != XmlNodeType.Element) 
            {
                reader.Read();
                continue;
            }

            var key = reader.GetAttribute("Key");
            var value = reader.GetAttribute("Value");

            if (key == null || value == null)
            {
                throw new XmlException("Invalid cache entry: missing key or value");
            }

            try
            {
                this.Add((TKey)Convert.ChangeType(key, typeof(TKey)), 
                        (TValue)Convert.ChangeType(value, typeof(TValue)));
            }
            catch (Exception ex) when (ex is InvalidCastException or FormatException)
            {
                throw new XmlException($"Failed to convert cache entry: {key} => {value}", ex);
            }

            reader.Read();
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        if (writer == null) throw new ArgumentNullException(nameof(writer));

        foreach (var kvp in this)
        {
            writer.WriteStartElement("Entry");
            writer.WriteAttributeString("Key", kvp.Key?.ToString());
            writer.WriteAttributeString("Value", kvp.Value?.ToString());
            writer.WriteEndElement();
        }
    }
} 