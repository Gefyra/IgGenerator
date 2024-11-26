using IgGenerator.ConsoleHandling;
using IgGenerator.ConsoleHandling.Interfaces;

namespace IgGenerator.IgHandling;

public class NamingManipulationHandler(IUserInteractionHandler userInteractionHandler) : INamingManipulationHandler
{
    private bool DoNamingManipulation { get; set; }
    private string[]? filterPartFromFilename;


    public void StartConsoleWorkflow()
    {
        DoNamingManipulation = userInteractionHandler.AskYesNoQuestion("Do you want to manipulate names? (default: false)", false);
    }

    public string FilterPartFromFilename(string filename)
    {
        if (!DoNamingManipulation) return filename;
        filterPartFromFilename ??= userInteractionHandler.GetString("Part to filter from filename (comma-separated list - no whitespace!):").Split(',');
        return filterPartFromFilename.Aggregate(filename, (current, filter) => current.Replace(filter, ""));
    }

}

public interface INamingManipulationHandler
{
    public void StartConsoleWorkflow();
    public string FilterPartFromFilename(string filename);
}