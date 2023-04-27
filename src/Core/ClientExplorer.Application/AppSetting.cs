using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ClientExplorer.Application;

/// <summary>
/// Класс настроек приложения
/// </summary>
public class AppSetting
{
  #region Public Properties

  /// <summary>
  /// Путь до общей папки клиентов.
  /// Необходимо указывать путь в зависимости от типа операционной системы.
  /// </summary>
  public string CurrentPath { get; set; }

  /// <summary>
  /// Имя папки хранения различных ресурсов приложения.
  /// Она хранится в общей папке клиентов.
  /// </summary>
  public string OnClientPathDataResourceAppFolderName { get; set; }

  /// <summary>
  /// Имя папки хранения активов приложения.
  /// Это вложенная папка, находится в папке ресурсов.
  /// </summary>
  public string OnClientPathAssetsFolderName { get; set; }

  /// <summary>
  /// Имя файла, в котором хранится база адресов объектов
  /// </summary>
  public string AddressLocationsSourceFileName { get; set; }

  /// <summary>
  /// Имя для папки 'Объекты'
  /// </summary>
  public string FolderObjectsEntityName { get; set; }

  /// <summary>
  /// Имя файла логотипа клиента.
  /// Если в папке клиента найден файл с таким именем, этот файл будет использоваться как иконка.
  /// Файл должен быть *.png 
  /// </summary>
  public string ClientLogoFileName { get; set; }

  #endregion

  #region Constructor

  /// <summary>
  /// Конструктор. Инициализация свойств
  /// </summary>
  public AppSetting()
  {
    CurrentPath = string.Empty;
    OnClientPathDataResourceAppFolderName = string.Empty;
    OnClientPathAssetsFolderName = string.Empty;
    AddressLocationsSourceFileName = string.Empty;
    FolderObjectsEntityName = string.Empty;
    ClientLogoFileName = string.Empty;
  }

  #endregion

  #region Public Methods

  /// <summary>
  /// Чтение настроек из файла.
  /// </summary>
  /// <returns>
  /// Возвращает тип AppSetting
  /// </returns>
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

  /// <summary>
  /// Сохранение настроек в файл
  /// </summary>
  /// <param name="filePath">
  /// Имя файла, включая расщирение.
  /// </param>
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

  /// <summary>
  /// Устанавливает настройки по умолчанию.
  /// </summary>
  private void DefaultSettings()
  {
    OnClientPathDataResourceAppFolderName = ".ClientExplorer";
    OnClientPathAssetsFolderName = "Assets";

    AddressLocationsSourceFileName = "AddressLocations.json";

    FolderObjectsEntityName = "Объекты";

    CurrentPath = "/mnt/share/Clients";

    ClientLogoFileName = ".logo.png";
  }

  #endregion
}