using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ClientExplorer.Shared.ViewModels;

/// <summary>
/// Работа с форматом JSON
/// </summary>
public class JsonHelper
{
  
  /*
  /// <summary>
  /// Сохраняет список объектов клиента в файл
  /// </summary>
  /// <param name="loc">Список объектов клиента</param>
  public static async void LocationsToJSON(ListLocations loc)
  {
    string fileName = AppData.DEFAULT_FOLDER_PATH + Path.DirectorySeparatorChar + AppData.LOCATIONS_FILE_NAME;
  
    if (AppData.Locations == null) return;
  
    if (!Directory.Exists(AppData.DEFAULT_FOLDER_PATH))
    {
      Directory.CreateDirectory(AppData.DEFAULT_FOLDER_PATH);
    }
  
    JsonSerializerOptions options = new JsonSerializerOptions
    {
      Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
      WriteIndented = true
    };
  
    using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
    {
      await JsonSerializer.SerializeAsync<ListLocations>(fs, AppData.Locations, options);
    }
  }

  /// <summary>
  /// Получает список объектов клиента из подготовленного файла
  /// </summary>
  public static void LocationsFromJSON()
  {
    String fileName = AppData.DEFAULT_FOLDER_PATH + Path.DirectorySeparatorChar + AppData.LOCATIONS_FILE_NAME;
  
    //ListLocations? locations = new ListLocations();
  
    using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read))
    {
      //AppData.Locations = await JsonSerializer.DeserializeAsync<ListLocations>(fs); //Это не работало
      AppData.Locations = JsonSerializer.Deserialize<ListLocations>(fs);
  
      //MessageBox.Show(string.Format("Count: {0}", AppData.Locations.Locations.Count)); //Тут поле пустое == null
      //ListLocations? loc = await JsonSerializer.DeserializeAsync<ListLocations>(fs);
      //Console.WriteLine(loc.Locations.Count.ToString());
    }
  }
  */
  
}