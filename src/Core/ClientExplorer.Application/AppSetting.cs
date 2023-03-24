using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ClientExplorer.Application;

public class AppSetting
{
  public string CurrentPath { get; set; }
  public string OnClientPathDataResourceAppFolderName { get; set; }
  public string OnClientPathAssetsFolderName { get; set; }
  public string AddressLocationsSourceFileName { get; set; }
  public string FolderObjectsEntityName { get; set; }
  public string ClientLogoFileName { get; set; }

  public AppSetting()
  {
    CurrentPath = string.Empty;
    OnClientPathDataResourceAppFolderName = string.Empty;
    OnClientPathAssetsFolderName = string.Empty;
    AddressLocationsSourceFileName = string.Empty;
    FolderObjectsEntityName = string.Empty;
    ClientLogoFileName = string.Empty;
  }

  public AppSetting LoadSettings()
  {
    var filePath = "Assets" + Path.DirectorySeparatorChar;
    filePath += "Settings.json";

    if (File.Exists(filePath))
    {
      using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
      var json = JsonSerializer.Deserialize<AppSetting>(fs);
      if (json != null)
      {
        return json;
      }
    }

    return new AppSetting();
  }


  private void UploadSettings(string filePath)
  {
    var options = new JsonSerializerOptions
    {
      Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
      WriteIndented = true
    };

    if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

    using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
    JsonSerializer.Serialize(fs, this, options);
  }

  private void DefaultSettings()
  {
    OnClientPathDataResourceAppFolderName = ".ClientExplorer";
    OnClientPathAssetsFolderName = "Assets";

    AddressLocationsSourceFileName = "AddressLocations.json";

    FolderObjectsEntityName = "Объекты";

    CurrentPath = "/mnt/share/Clients";

    ClientLogoFileName = ".logo.png";
  }
}