namespace IgGenerator.Helpers;

public static class Extensions
{
    public static string? FindFolderPath(this DirectoryInfo directoryInfo, string folderToFind)
    {
        if (directoryInfo.Name.Equals(folderToFind, StringComparison.OrdinalIgnoreCase))
        {
            return directoryInfo.FullName;
        }

        return directoryInfo
            .GetDirectories()
            .Select(subDirectory => FindFolderPath(subDirectory, folderToFind))
            .FirstOrDefault(foundPath => !string.IsNullOrEmpty(foundPath));
    }
}