using System.Collections.ObjectModel;
using System.Text.Json;
using ClientExplorer.Shared.ViewModels;

namespace ClientExplorer.Application;

public static class ClientEr
{
  public const string DefaultDataResourcePath = ".ClientExplorer.Assets";
  public const string LocationsSourceFileName = ".AddressLocations.json";

  public const string FolderObjectsName = "Объекты";

  public static string? CurrentPath { get; set; }

  public static readonly DirectoriesInClient DirectoriesInClient = new DirectoriesInClient();
  public static readonly DirectoriesInLocation DirectoriesInLocation = new DirectoriesInLocation();

  /*
   public  static async Task<ObservableCollection<LocationViewModel>?> GetLocations()
  {
      string fileName = _defaultDataResourcePath + Path.DirectorySeparatorChar + _locationsSourceFileName;

      ObservableCollection<LocationViewModel>? locations;

      using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read))
      {
          //AppData.Locations = await JsonSerializer.DeserializeAsync<ListLocations>(fs); //Это не работало
          locations = await JsonSerializer.DeserializeAsync<ObservableCollection<LocationViewModel>>(fs);

          //MessageBox.Show(string.Format("Count: {0}", AppData.Locations.Locations.Count)); //Тут поле пустое == null
          //ListLocations? loc = await JsonSerializer.DeserializeAsync<ListLocations>(fs);
          //Console.WriteLine(loc.Locations.Count.ToString());
      }
      return locations;
  }*/
}