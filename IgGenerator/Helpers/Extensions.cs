using System.Text.RegularExpressions;
using IgGenerator.DataObjectHandling.Interfaces;

namespace IgGenerator.Helpers;

public static class Extensions
{
    public static string? FindFolderPath(this DirectoryInfo directoryInfo, string folderToFind)
    {

            return directoryInfo.FindFolderPathDirectoryInfo(folderToFind)?.FullName;
    }
    
    public static DirectoryInfo? FindFolderPathDirectoryInfo(this DirectoryInfo directoryInfo, string folderToFind)
    {
        if (directoryInfo.Name.Equals(folderToFind, StringComparison.OrdinalIgnoreCase))
        {
            return directoryInfo;
        }

        return directoryInfo
            .GetDirectories()
            .Select(subDirectory => FindFolderPathDirectoryInfo(subDirectory, folderToFind))
            .FirstOrDefault(info => info is not null);
    }
    
    public static string? GetPathFromFolder(this string filePath, string folderName)
    {
        int index = filePath.IndexOf(folderName, StringComparison.OrdinalIgnoreCase);
        return index >= 0 ? filePath[index..] : null;
    }

    public static string ChangeUmlaut(this string str)
    {
        return str
            .Replace("ue", "ü")
            .Replace("Ue", "Ü")
            .Replace("oe", "ö")
            .Replace("Oe", "Ö")
            .Replace("ae", "ä")
            .Replace("Ae", "Ä");
    }

    public static string RemoveEmptyLines(this string input) 
        => string.Join("\n", input.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)));
    
    public static string ReplaceVars(this string content) =>
        content
            .Replace(IVariable.STARTTOCOBJECT, "")
            .Replace(IVariable.ENDTOCOBJECT, "")
            .Replace("$$", "");
}