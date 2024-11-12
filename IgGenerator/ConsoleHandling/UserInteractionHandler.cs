using Hl7.FhirPath.Expressions;

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
        } while(string.IsNullOrEmpty(answer) && bool.TryParse(answer, out _));
        return bool.Parse(answer!);
    }
}