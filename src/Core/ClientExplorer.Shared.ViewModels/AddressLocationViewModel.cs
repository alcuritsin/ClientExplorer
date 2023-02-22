using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using ClientExplorer.Application;

namespace ClientExplorer.Shared.ViewModels;

public class AddressLocationViewModel : BaseViewModel
{
  #region Public Properties

  public ObservableCollection<AddressLocationEntityViewModel>? AddressLocations { get; private set; } =
    new ObservableCollection<AddressLocationEntityViewModel>();

  #endregion

  #region Constructor

  public AddressLocationViewModel()
  {
    _directoryDataResourcePath =
      ClientEr.CurrentPath + Path.DirectorySeparatorChar + ClientEr.DefaultDataResourcePath;
    
    _locationsFilePath = _directoryDataResourcePath + Path.DirectorySeparatorChar + ClientEr.LocationsSourceFileName;
    
    LoadAddressLocations();

    // Упорядочить список по Алфавиту. Город+Улица+НомерДома.
    List<AddressLocForOrdered> addressLocs = new System.Collections.Generic.List<AddressLocForOrdered>();

    foreach (var addressLocation in AddressLocations)
    {
      addressLocs.Add(new AddressLocForOrdered(addressLocation));
    }

    var addressLocsOrdered = addressLocs.OrderBy(i => i.OrderProp);

    AddressLocations.Clear();

    foreach (var addressLoc in addressLocsOrdered)
    {
      AddressLocations.Add(new AddressLocationEntityViewModel()
      {
        CityName = addressLoc.Loc.CityName,
        StreetName = addressLoc.Loc.StreetName,
        HouseNumber = addressLoc.Loc.HouseNumber
      });
    }
  }

  #endregion

  #region Private Clases

  /// <summary>
  /// Приватный класс для возможности упорядочить список объектов (локаций) клиента 
  /// </summary>
  private class AddressLocForOrdered
  {
    public AddressLocationEntityViewModel Loc { get; }
    public string OrderProp { get; }

    public AddressLocForOrdered(AddressLocationEntityViewModel loc)
    {
      Loc = loc;
      OrderProp = loc.CityName.ToUpper() + "~" + loc.StreetName.ToUpper() + "~" + loc.HouseNumber.ToUpper();
    }
  }

  #endregion

  #region Private Properties

  private readonly string _locationsFilePath;
  private readonly string _directoryDataResourcePath;

  #endregion

  #region Private Methods

  /// <summary>
  /// Загрузка базы адресов из файла
  /// </summary>
  private void LoadAddressLocations()
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

  /// <summary>
  /// Выгрузка текущего списка адресов в файл  
  /// </summary>
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

    await using FileStream fs = new FileStream(_locationsFilePath, FileMode.OpenOrCreate);
    if (AddressLocations != null)
      await JsonSerializer.SerializeAsync<ObservableCollection<AddressLocationEntityViewModel>>(fs, AddressLocations,
        options);
  }

  #endregion
}