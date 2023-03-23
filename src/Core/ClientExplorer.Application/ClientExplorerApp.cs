using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ClientExplorer.Application;

public static class ClientExplorerApp
{
  public const string VersionApp = "ver: 1.0.0";
  public const string DefaultDataResourcePath = ".ClientExplorer";
  public static readonly string LocationsSourceFileName = "Assets" + Path.DirectorySeparatorChar + "AddressLocations.json";

  public const string FolderObjectsName = "Объекты";

  public static string? CurrentPath { get; set; }

  public static readonly DirectoriesInClient DirectoriesInClient = new DirectoriesInClient();
  public static readonly DirectoriesInLocation DirectoriesInLocation = new DirectoriesInLocation();

  public static ClientExplorerSetting Settings = new ClientExplorerSetting();
}

public class ClientExplorerSetting
{
  //TODO Вынос всех настроек в файл "setting.json"
  public string CurrentPath { get; set; }
  public string DataResourcePathName { get; set; }
  public string AssetsPathName { get; set; }
  public string AddressLocationsSourceFileName { get; set; }
  public string FolderObjectsName { get; set; }
  
  public ClientExplorerSetting()
  {
    // Путь к файлу настроек - './Assets/Settings.json'
    var filePath = "." + Path.DirectorySeparatorChar;
    filePath += "Assets" + Path.DirectorySeparatorChar;
    filePath += "Settings.json";
    
    DataResourcePathName = ".ClientExplorer";
    AssetsPathName = "Assets";

    AddressLocationsSourceFileName = "AddressLocations.json";

    FolderObjectsName = "Объекты";

    CurrentPath = "/mnt/share/Clients";
  }

  private void LoadSettings(string filePath)
  {
    
   
    
  }
  
  private void UploadSettings(string filePath)
  {
    var setting = new ClientExplorerSetting();
    
    var options = new JsonSerializerOptions
    {
      Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
      WriteIndented = true
    };
    
    if (!Directory.Exists("./Assets")) Directory.CreateDirectory("./Assets");
    using var fs = new FileStream("./Assets/Setting.json", FileMode.Create, FileAccess.Write);
    JsonSerializer.Serialize(fs, this, options);

  }
  
}