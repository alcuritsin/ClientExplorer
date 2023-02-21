using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using ClientExplorer.Application;

namespace ClientExplorer.Shared.ViewModels;

public class AddressLocationViewModel : BaseViewModel
{
  private class AddressLocForOrdered
  {
    public AddressLocationEntityViewModel Loc { get; }
    public string OrderProp { get; }

    public AddressLocForOrdered(AddressLocationEntityViewModel _loc)
    {
      Loc = _loc;
      OrderProp = _loc.CityName.ToUpper() + "~" + _loc.StreetName.ToUpper() + "~" + _loc.HouseNumber.ToUpper();
    }
  }

  public AddressLocationViewModel()
  {
    _directoryDataResourcePath =
      ClientEr.CurrentPath + Path.DirectorySeparatorChar + ClientEr.DefaultDataResourcePath;
    _locationsFilePath = _directoryDataResourcePath + Path.DirectorySeparatorChar + ClientEr.LocationsSourceFileName;
    FillAddressLocations();

    // Упорядочить список по Алфавиту. Город+Улица+НомерДома.
    List<AddressLocForOrdered> addressLocsForOrdered = new System.Collections.Generic.List<AddressLocForOrdered>();

    foreach (var addressLocation in AddressLocations)
    {
      addressLocsForOrdered.Add(new AddressLocForOrdered(addressLocation));
    }

    addressLocsForOrdered.OrderBy(i => i.OrderProp);

    AddressLocations.Clear();

    foreach (var addressLoc in addressLocsForOrdered)
    {
      AddressLocations.Add(new AddressLocationEntityViewModel()
      {
        CityName = addressLoc.Loc.CityName,
        StreetName = addressLoc.Loc.StreetName,
        HouseNumber = addressLoc.Loc.HouseNumber
      });
    }
  }

  public ObservableCollection<AddressLocationEntityViewModel>? AddressLocations { get; private set; } =
    new ObservableCollection<AddressLocationEntityViewModel>();

  private readonly string _locationsFilePath;
  private readonly string _directoryDataResourcePath;

  private void FillAddressLocations()
  {
    using FileStream fs = new FileStream(_locationsFilePath, FileMode.OpenOrCreate, FileAccess.Read);
    AddressLocations = JsonSerializer.Deserialize<ObservableCollection<AddressLocationEntityViewModel>>(fs);
  }
  // private async Task FillAddressLocations()
  // {
  //     using (FileStream fs = new FileStream(_locationsFilePath, FileMode.OpenOrCreate, FileAccess.Read))
  //     {
  //         //AppData.Locations = await JsonSerializer.DeserializeAsync<ListLocations>(fs); //Это не работало
  //         AddressLocations = await JsonSerializer.DeserializeAsync<ObservableCollection<AddressLocationEntityViewModel>>(fs);
  //
  //         //MessageBox.Show(string.Format("Count: {0}", AppData.Locations.Locations.Count)); //Тут поле пустое == null
  //         //ListLocations? loc = await JsonSerializer.DeserializeAsync<ListLocations>(fs);
  //         //Console.WriteLine(loc.Locations.Count.ToString());
  //     }
  // }

  private async Task UploadAddressLocations()
  {
    if (!Directory.Exists(_directoryDataResourcePath))
    {
      Directory.CreateDirectory(_directoryDataResourcePath);
    }

    JsonSerializerOptions options = new JsonSerializerOptions
    {
      Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
      WriteIndented = true
    };

    using (FileStream fs = new FileStream(_locationsFilePath, FileMode.OpenOrCreate))
    {
      if (AddressLocations != null)
        await JsonSerializer.SerializeAsync<ObservableCollection<AddressLocationEntityViewModel>>(fs, AddressLocations,
          options);
    }
  }
}