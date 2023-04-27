namespace ClientExplorer.Application;

public static class ClientExplorerApp
{
  public const string VersionApp = "ver: 1.0.0";
  public static readonly AppSetting Settings = new AppSetting().LoadSettings();

  public static readonly DirectoriesInClient DirectoriesInClient = new DirectoriesInClient();
  public static readonly DirectoriesInLocation DirectoriesInLocation = new DirectoriesInLocation();


  /// <summary>
  /// Возвращает путь к файлу хранения коллекции адресов (локаций).
  /// </summary>
  /// <returns>
  /// Путь к файлу коллекции адресов (локаций)
  /// </returns>
  public static string GetLocationsSourceFilePath()
  {
    var locationsFilePath = GetOnClientPathAssetsFolderPath();
    locationsFilePath += Settings.AddressLocationsSourceFileName;

    return locationsFilePath;
  }

  /// <summary>
  /// Возвращает путь к папке хранения служебных данных приложения, в папке клиентов.
  /// </summary>
  /// <returns>
  /// Путь к папке хранения служебных данных приложения.
  /// </returns>
  public static string GetOnClientPathDataResourceAppPath()
  {
    var onClientPathDataResourceAppPath = Settings.CurrentPath + Path.DirectorySeparatorChar;
    onClientPathDataResourceAppPath += Settings.OnClientPathDataResourceAppFolderName + Path.DirectorySeparatorChar;

    return onClientPathDataResourceAppPath;
  }

  /// <summary>
  /// Возвращает путь к папке хранения ресурсов приложения, в папке клиентов.
  /// </summary>
  /// <returns>
  /// Путь к папке хранения служебных данных приложения.
  /// </returns>
  public static string GetOnClientPathAssetsFolderPath()
  {
    var onClientPathAssetsFolderPath = GetOnClientPathDataResourceAppPath();
    onClientPathAssetsFolderPath += Settings.OnClientPathAssetsFolderName + Path.DirectorySeparatorChar;

    return onClientPathAssetsFolderPath;
  }
}