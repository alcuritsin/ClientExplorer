using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using ClientExplorer.Application;

namespace ClientExplorer.Shared.ViewModels;

public class LocationViewModel : BaseViewModel
{
    public LocationViewModel()
    {
        _locationsFilePath = ClientEr.DefaultDataResourcePath + Path.DirectorySeparatorChar +
                             ClientEr.LocationsSourceFileName;
    }

    
    
    
    public ObservableCollection<LocationEntityViewModel>? Locations { get; set; } =
        new ObservableCollection<LocationEntityViewModel>();

    private readonly string _locationsFilePath;

    private async Task FillLocations()
    {
        using (FileStream fs = new FileStream(_locationsFilePath, FileMode.OpenOrCreate, FileAccess.Read))
        {
            //AppData.Locations = await JsonSerializer.DeserializeAsync<ListLocations>(fs); //Это не работало
            Locations = await JsonSerializer.DeserializeAsync<ObservableCollection<LocationEntityViewModel>>(fs);

            //MessageBox.Show(string.Format("Count: {0}", AppData.Locations.Locations.Count)); //Тут поле пустое == null
            //ListLocations? loc = await JsonSerializer.DeserializeAsync<ListLocations>(fs);
            //Console.WriteLine(loc.Locations.Count.ToString());
        }
    }

    private async Task UploadLocations()
    {
        if (!Directory.Exists(_locationsFilePath))
        {
            Directory.CreateDirectory(_locationsFilePath);
        }

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            WriteIndented = true
        };

        using (FileStream fs = new FileStream(_locationsFilePath, FileMode.OpenOrCreate))
        {
            if (Locations != null)
                await JsonSerializer.SerializeAsync<ObservableCollection<LocationEntityViewModel>>(fs, Locations,
                    options);
        }
    }
}