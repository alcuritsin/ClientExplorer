using System.Collections.ObjectModel;
using System.Text.Json;
using ClientExplorer.Shared.ViewModels;

namespace ClientExplorer.Application;

public static class ClientEr
{
    public static string DefaultDataResourcePath = ".ClientExplorer.Assets";
    public static string LocationsSourceFileName = ".AddressLocations.json";
    public static string? CurrentPath { get; set; }

    public static ClientDirectories ClientDirectories = new ClientDirectories();
    public static LocationDirectories LocationDirectories = new LocationDirectories();

    public static string GetPathDirectoryEntity(DirectoryEntity dir)
    {
        if (dir.ParentDir == null) return Path.DirectorySeparatorChar + dir.DirName;

        return GetPathDirectoryEntity(dir.ParentDir);
    }
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