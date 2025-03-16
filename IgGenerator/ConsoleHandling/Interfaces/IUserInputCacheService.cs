namespace IgGenerator.ConsoleHandling.Interfaces;

/// <summary>
/// Interface for caching and retrieving user input.
/// </summary>
public interface IUserInputCacheService
{
    /// <summary>
    /// Tries to get a cached value for the given question.
    /// </summary>
    /// <param name="question">The question to look up</param>
    /// <param name="value">The cached value if found</param>
    /// <returns>True if a cached value was found, false otherwise</returns>
    bool TryGetCachedValue(string question, out string value);

    /// <summary>
    /// Adds or updates a cache entry.
    /// </summary>
    /// <param name="question">The question to cache</param>
    /// <param name="answer">The answer to cache</param>
    void AddOrUpdateCache(string question, string answer);

    /// <summary>
    /// Displays the current cache contents and asks if it should be used.
    /// </summary>
    /// <param name="askUser">Function to ask the user a yes/no question</param>
    void ConfigureCache(Func<string, bool, bool> askUser);
} 