namespace IgGenerator.ConsoleHandling;

public interface IUserInteractionHandler
{
    public int GetNumber(string question, int defaultAnswer);
    public string GetString(string question);
    public void Send(string message);
    public void SendAndExit(string message);
    public bool AskYesNoQuestion(string question, bool defaultAnswer);
    public void AskCacheUsage();
}