namespace ClientExplorer.Application;

public static class ClientEr
{
  public const string DefaultDataResourcePath = ".ClientExplorer";
  public static readonly string LocationsSourceFileName = "Assets" + Path.DirectorySeparatorChar + "AddressLocations.json";

  public const string FolderObjectsName = "Объекты";

  public static string? CurrentPath { get; set; }

  public static readonly DirectoriesInClient DirectoriesInClient = new DirectoriesInClient();
  public static readonly DirectoriesInLocation DirectoriesInLocation = new DirectoriesInLocation();

}