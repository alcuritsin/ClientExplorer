using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using ClientExplorer.Application;

namespace ClientExplorer.Shared.ViewModels;

public class AddressLocationViewModel : BaseViewModel
{
  #region Public Properties

  public ObservableCollection<AddressLocationEntityViewModel> AddressLocations { get; private set; }

  #endregion

  #region Public Methods

  /// <summary>
  /// Сохраняет адрес в файле адресного класификатора, если этого адреса там не было.
  /// </summary>
  /// <param name="city">
  /// Наименование населённого пункта
  /// </param>
  /// <param name="street">
  /// Наименование улици
  /// </param>
  /// <param name="houseNumber">
  /// Номер дома
  /// </param>
  public bool SaveLocationIfNew(string city, string street, string houseNumber)
  {
    var isNew = true;

    foreach (var addressLocation in AddressLocations)
    {
      if (addressLocation.CityName == city &&
          addressLocation.StreetName == street &&
          addressLocation.HouseNumber == houseNumber)
      {
        isNew = false;
        break;
      }
    }

    if (isNew)
    {
      var location = new AddressLocationEntityViewModel()
      {
        CityName = city,
        StreetName = street,
        HouseNumber = houseNumber
      };

      AddressLocations.Add(location);

      OrderLocation();

      UploadAddressLocations();
    }

    return isNew;
  }

  #endregion

  #region Constructor

  public AddressLocationViewModel()
  {
    AddressLocations = new ObservableCollection<AddressLocationEntityViewModel>();

    _directoryDataResourcePath =
      ClientEr.CurrentPath + Path.DirectorySeparatorChar + ClientEr.DefaultDataResourcePath;

    _locationsFilePath = _directoryDataResourcePath + Path.DirectorySeparatorChar + ClientEr.LocationsSourceFileName;

    if (!Directory.Exists(_directoryDataResourcePath))
    {
      return;
    }

    LoadAddressLocations();

    UniqLocation();

    OrderLocation();

    UploadAddressLocations();
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
    if (Directory.Exists(_locationsFilePath))
    {
      using FileStream fs = new FileStream(_locationsFilePath, FileMode.Open, FileAccess.Read);
      var json = JsonSerializer.Deserialize<ObservableCollection<AddressLocationEntityViewModel>>(fs);
      if (json != null) AddressLocations = json;
    }
  }

  /// <summary>
  /// Выгрузка текущего списка адресов в файл  
  /// </summary>
  private void UploadAddressLocations()
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

    using FileStream fs = new FileStream(_locationsFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
    JsonSerializer.Serialize(fs, AddressLocations, options);
  }

  /// <summary>
  /// Упорядочивает список по Алфавиту. Город+Улица+НомерДома.
  /// </summary>
  private void OrderLocation()
  {
    List<AddressLocForOrdered> addressLocs = new List<AddressLocForOrdered>();

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
        CityName = addressLoc.Loc.CityName.Trim(),
        StreetName = addressLoc.Loc.StreetName.Trim(),
        HouseNumber = addressLoc.Loc.HouseNumber.Trim()
      });
    }
  }

  private void UniqLocation()
  {
    List<AddressLocationEntityViewModel> addressLocsBuffer = AddressLocations.ToList();

    AddressLocations.Clear();

    foreach (var addressLocBuff in addressLocsBuffer)
    {
      var isNewLocation = true;

      foreach (var addressLocation in AddressLocations)
      {
        if (Equals(addressLocation.CityName, addressLocBuff.CityName) &&
            Equals(addressLocation.StreetName, addressLocBuff.StreetName) &&
            Equals(addressLocation.HouseNumber, addressLocBuff.HouseNumber))
        {
          isNewLocation = false;
          break;
        }
      }

      if (isNewLocation)
      {
        AddressLocations.Add(addressLocBuff);
      }
    }
  }

  #endregion
}