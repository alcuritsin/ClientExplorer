namespace ClientExplorer.Shared.ViewModels;

/// <summary>
/// Работа с файловой системой
/// </summary>
public class FileSystemHelper
{
  /// <summary>
  /// Получить список папок по указанному пути
  /// </summary>
  /// <param name="path">Путь</param>
  /// <returns>Список папок</returns>
  public static List<String> GetListFolders(String path = "*")
  {
    String currentPath = Environment.CurrentDirectory;
    if (path != "*") currentPath = path;

    List<String> folders = new List<String>();

    // Make a reference to a directory.
    DirectoryInfo di = new DirectoryInfo(currentPath);

    // Get a reference to each directory in that directory.
    DirectoryInfo[] diArr = di.GetDirectories();

    // Display the names of the directories.
    foreach (DirectoryInfo dri in diArr)
      folders.Add(dri.Name);

    return folders;
  }
}