namespace IgGenerator.ConsoleHandling;

public class UserInteractionHandler : IUserInteractionHandler
{
    public int GetNumber(string question, int defaultAnswer)
    {
        Console.WriteLine(question);
        return int.Parse(Console.ReadLine() ?? $"{defaultAnswer}");
    }

    public string GetString(string question)
    {
        Console.WriteLine(question);
        string? folder;
        do
        {
            folder = Console.ReadLine();
        } while(string.IsNullOrEmpty(folder));
        return folder;
    }
}

public interface IUserInteractionHandler
{
    public int GetNumber(string question, int defaultAnswer);
    public string GetString(string question);
}