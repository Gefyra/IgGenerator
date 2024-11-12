using IgGenerator.ConsoleHandling;

namespace IgGenerator.IgHandling;

public class NamingManipulationHandler(IUserInteractionHandler userInteractionHandler) : INamingManipulationHandler
{
    private bool DoNamingManipulation { get; set; }
    private string? filterPartFromFilename;


    public void StartConsoleWorkflow()
    {
        DoNamingManipulation = userInteractionHandler.AskYesNoQuestion("Do you want to switch off naming manipulation? (default: false)", false);
    }

    public string FilterPartFromFilename(string filename)
    {
        string result = filename;
        if (!DoNamingManipulation) return result;
        filterPartFromFilename ??= userInteractionHandler.GetString("Part to filter from filename:");
        result = result.Replace(filterPartFromFilename, "");
        return result;
    }

}

public interface INamingManipulationHandler
{
    public void StartConsoleWorkflow();
    public string FilterPartFromFilename(string filename);
}