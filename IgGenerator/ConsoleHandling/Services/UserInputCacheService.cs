using System.Xml.Serialization;
using IgGenerator.ConsoleHandling.Interfaces;
using IgGenerator.ConsoleHandling.Models;

namespace IgGenerator.ConsoleHandling.Services;

/// <summary>
/// Service for caching and retrieving user input.
/// </summary>
public sealed class UserInputCacheService : IUserInputCacheService
{
    private const string CacheFilePath = "./cache.xml";
    private readonly DictionaryXmlable<string, string> _cache;
    private bool _useCache;

    public UserInputCacheService()
    {
        _cache = RestoreCache();
        _useCache = true;
    }

    /// <summary>
    /// Tries to get a cached value for the given question.
    /// </summary>
    /// <param name="question">The question to look up</param>
    /// <param name="value">The cached value if found</param>
    /// <returns>True if a cached value was found, false otherwise</returns>
    public bool TryGetCachedValue(string question, out string value)
    {
        value = string.Empty;
        return _useCache && _cache.TryGetValue(question, out value);
    }

    /// <summary>
    /// Adds or updates a cache entry.
    /// </summary>
    /// <param name="question">The question to cache</param>
    /// <param name="answer">The answer to cache</param>
    public void AddOrUpdateCache(string question, string answer)
    {
        if (string.IsNullOrEmpty(question))
            throw new ArgumentNullException(nameof(question));
        if (string.IsNullOrEmpty(answer))
            throw new ArgumentNullException(nameof(answer));

        if (_cache.ContainsKey(question))
        {
            _cache[question] = answer;
        }
        else
        {
            _cache.Add(question, answer);
        }

        SaveCache();
    }

    /// <summary>
    /// Displays the current cache contents and asks if it should be used.
    /// </summary>
    /// <param name="askUser">Function to ask the user a yes/no question</param>
    public void ConfigureCache(Func<string, bool, bool> askUser)
    {
        if (askUser == null) throw new ArgumentNullException(nameof(askUser));

        var cacheContent = _cache.Aggregate(
            string.Empty,
            (current, kvp) => current + $"{kvp.Key} => {kvp.Value}{Environment.NewLine}"
        );

        Console.WriteLine(cacheContent);
        _useCache = askUser("Use Cache?", true);
    }

    private void SaveCache()
    {
        try
        {
            var serializer = new XmlSerializer(typeof(DictionaryXmlable<string, string>));
            using var writer = new StreamWriter(CacheFilePath);
            serializer.Serialize(writer, _cache);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to save cache: {ex.Message}");
        }
    }

    private static DictionaryXmlable<string, string> RestoreCache()
    {
        if (!File.Exists(CacheFilePath))
        {
            return new DictionaryXmlable<string, string>();
        }

        try
        {
            var serializer = new XmlSerializer(typeof(DictionaryXmlable<string, string>));
            using var reader = new StreamReader(CacheFilePath);
            return (DictionaryXmlable<string, string>)serializer.Deserialize(reader)!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to restore cache: {ex.Message}");
            return new DictionaryXmlable<string, string>();
        }
    }
} 