using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Input;
using ClientExplorer.Application;

namespace ClientExplorer.Shared.ViewModels;

public class MainViewModel : BaseViewModel
{
  #region Public Properties

  //Info панель
  public string StatusInfo { get; set; }

  #endregion

  //Клиент

  #region Client

  #region Public Properties

  public string ClientFilter { get; set; }
  public ObservableCollection<ClientEntityViewModel> SortedClients { get; set; }
  public ClientEntityViewModel SelectedClient { get; set; }

  #endregion

  #region Events

  public ICommand OpenClient { get; }
  public ICommand KeyUpClientName { get; }

  #endregion

  #region Commands Methods

  private void SelectClient(object param)
  {
    LoadClientLocation();
  }

  /// <summary>
  /// Применение фильтра к 'Списку Клиентов' (SortedClients). На основании (ClientFilter).
  /// </summary>
  /// <param name="param">
  /// Строка фильтр
  /// </param>
  private void ApplyFilterToClientsList(object param)
  {
    var clientFilter = ClientFilter;

    StatusInfo = "ClientFilter: " + clientFilter;

    // Пустой фильтр
    if (Equals(clientFilter, string.Empty))
    {
      SortedClients.Clear();

      foreach (var client in _clientsList)
      {
        SortedClients.Add(client);
      }

      StatusInfo += " SortedClients.Count: " + SortedClients.Count;

      return;
    }

    if (IsNameHaveInvalidCharacter(ref clientFilter))
    {
      ClientFilter = clientFilter;
      return;
    }

    SortedClients.Clear();

    foreach (var client in _clientsList)
    {
      if (client.Name == null)
        throw new Exception("Err: " + nameof(ApplyFilterToClientsList) + ". " + nameof(client.Name) + " = null");
      
      if (client.Name.ToUpper().Contains(clientFilter.ToUpper()))
      {
        SortedClients.Add(client);
      }
    }

    StatusInfo += " SortedClients.Count: " + SortedClients.Count;
  }

  #endregion

  #region Private Properties

  private List<ClientEntityViewModel> _clientsList = new List<ClientEntityViewModel>();

  #endregion

  #region Private Methods

  /// <summary>
  /// Первичное заполнение списка клиентов
  /// </summary>
  private void InitClientList()
  {
    if (ClientEr.CurrentPath == null)
      throw new Exception("Err: " + nameof(InitClientList) + ". " + nameof(ClientEr.CurrentPath) + " = null");

    SortedClients.Clear();

    var directoryInfo = new DirectoryInfo(ClientEr.CurrentPath);

    foreach (var directory in directoryInfo.GetDirectories())
    {
      //  Точка вначале - символ скрытости. Такие директории пропускать. 
      if (directory.Name.First() != '.')
      {
        SortedClients.Add(new ClientEntityViewModel(directory.Name, new DirectoryInfo(directory.FullName)));
      }
    }

    SortedClients = new ObservableCollection<ClientEntityViewModel>(SortedClients.OrderBy(i => i.Name));

    _clientsList = SortedClients.ToList();
  }

  #endregion

  #endregion

  //Существующие объекты клиента

  #region Location of Client

  #region Public Properties

  public ObservableCollection<string> SortedLocation { get; set; }

  #endregion

  #region Private Properties

  private AddressLocationViewModel _addressLocationViewModel;

  #endregion

  #region Private Methods

  /// <summary>
  /// Загружает существующие объекты у клиента из директории 'Объекты'
  /// </summary>
  private void LoadClientLocation()
  {
    SortedLocation.Clear();

    if (SelectedClient.ClientPath == null)
    {
      StatusInfo = "Err: SelectedClient.ClientPath == null";
      return;
    }

    //TODO Имя папки "объекты" нужно вывести в свойство!
    var directoryPath = SelectedClient.ClientPath.FullName + Path.DirectorySeparatorChar + "Объекты";

    if (!Directory.Exists(directoryPath))
    {
      StatusInfo = "Err: 'Объекты' не обнаружены";
      return;
    }

    var directoryInfo = new DirectoryInfo(directoryPath);

    foreach (var directory in directoryInfo.GetDirectories())
    {
      SortedLocation.Add(directory.Name);
    }

    SortedLocation = new ObservableCollection<string>(SortedLocation.OrderBy(i => i));

    StatusInfo = SelectedClient.ClientPath.FullName;
  }

  #endregion

  #endregion

  // Адреса объектов (локации)

  #region Address Location

  #region Public Properties

  public string CityName { get; set; } = string.Empty;
  public string SelectedCity { get; set; } = string.Empty;

  public string StreetName { get; set; } = string.Empty;
  public string SelectedStreet { get; set; } = string.Empty;

  public string HouseNumber { get; set; } = string.Empty;
  public string SelectedHouseNumber { get; set; } = string.Empty;

  public string AdditionalParam { get; set; } = string.Empty;

  public ObservableCollection<string> CitiesFiltered { get; set; } =
    new ObservableCollection<string>();

  public ObservableCollection<string> StreetsFiltered { get; set; } =
    new ObservableCollection<string>();

  public ObservableCollection<string> HouseNumbersFiltered { get; set; } =
    new ObservableCollection<string>();

  #endregion

  #region Events

  public ICommand KeyUpCityName { get; }
  public ICommand LostFocusCityName { get; }
  public ICommand TappedOnCityItem { get; }

  public ICommand KeyUpStreetName { get; }
  public ICommand LostFocusStreetName { get; }
  public ICommand TappedOnStreetItem { get; }

  public ICommand KeyUpHouseNumber { get; }
  public ICommand TappedOnHouseNumberItem { get; }

  #endregion

  #region Commands Methods

  /// <summary>
  /// Применение фильтра к 'Списку Населённых пунктов' (CitiesFiltered). На основании (CityName).
  /// </summary>
  /// <param name="param"></param>
  private void ApplyFilterToCitiesName(object param)
  {
    var cityFilter = CityName;

    StatusInfo = "CityFilter: " + cityFilter;

    if (Equals(cityFilter, string.Empty))
    {
      CitiesFiltered.Clear();

      foreach (var cityName in _citiesName)
      {
        CitiesFiltered.Add(cityName);
      }

      StatusInfo += " CitiesFiltered.Count: " + CitiesFiltered.Count;

      return;
    }

    if (IsNameHaveInvalidCharacter(ref cityFilter))
    {
      CityName = cityFilter;
      return;
    }

    CitiesFiltered.Clear();

    foreach (var cityName in _citiesName)
    {
      if (cityName.ToUpper().Contains(cityFilter.ToUpper()))
      {
        CitiesFiltered.Add(cityName);
      }
    }

    StatusInfo += " CitiesFiltered.Count: " + CitiesFiltered.Count;
  }

  /// <summary>
  /// Выбор города из списка в Popup. 
  /// </summary>
  /// <param name="param"></param>
  private void SelectCity(object param)
  {
    CityName = SelectedCity;
    // _lastSelectCityName = SelectedCity;
    ApplyFilterToCitiesName(param);
    InitStreetsName();
    ApplyFilterToStreetsName(null);
  }

  /// <summary>
  /// Заполнение списка улиц
  /// </summary>
  /// <param name="param"></param>
  private void FillStreetsName(object param)
  {
    InitStreetsName();
  }

  /// <summary>
  /// Применение фильтра к 'Списку Улиц' (StreetsFiltered). На основании (CityName).
  /// </summary>
  /// <param name="param"></param>
  private void ApplyFilterToStreetsName(object? param)
  {
    var streetFilter = StreetName;

    StatusInfo = "StreetsFiltered: " + streetFilter;

    if (Equals(streetFilter, string.Empty))
    {
      StreetsFiltered.Clear();

      foreach (var streetName in _streetsName)
      {
        StreetsFiltered.Add(streetName);
      }

      StatusInfo += " StreetsFiltered.Count: " + StreetsFiltered.Count;

      return;
    }

    if (IsNameHaveInvalidCharacter(ref streetFilter))
    {
      StreetName = streetFilter;
      return;
    }

    StreetsFiltered.Clear();

    foreach (var streetName in _streetsName)
    {
      if (streetName.ToUpper().Contains(streetFilter.ToUpper()))
      {
        StreetsFiltered.Add(streetName);
      }
    }

    StatusInfo += " StreetsFiltered.Count: " + StreetsFiltered.Count;
  }

  /// <summary>
  /// Выбор улицы из списка в Popup
  /// </summary>
  /// <param name="param"></param>
  private void SelectStreet(object param)
  {
    StreetName = SelectedStreet;
    ApplyFilterToStreetsName(param);
    InitHouseNumbers();
    ApplyFilterToHouseNumbers(null);
  }

  /// <summary>
  /// Заполнение списка номеров домов
  /// </summary>
  /// <param name="param"></param>
  private void FillHouseNumbers(object param)
  {
    InitHouseNumbers();
  }

  /// <summary>
  /// Применение фильтра к 'Списку номеров домов' (HouseNumbersFiltered). На основании (HouseNumber).
  /// </summary>
  /// <param name="param"></param>
  private void ApplyFilterToHouseNumbers(object? param)
  {
    var houseNumberFilter = HouseNumber;

    StatusInfo = "HouseNumberFilter: " + houseNumberFilter;

    if (Equals(houseNumberFilter, string.Empty))
    {
      HouseNumbersFiltered.Clear();

      foreach (var houseNumber in _houseNumbers)
      {
        HouseNumbersFiltered.Add(houseNumber);
      }

      StatusInfo += " HouseNumbersFiltered.Count: " + HouseNumbersFiltered.Count;

      return;
    }

    if (IsNameHaveInvalidCharacter(ref houseNumberFilter))
    {
      HouseNumber = houseNumberFilter;
      return;
    }

    HouseNumbersFiltered.Clear();

    foreach (var houseNumber in _houseNumbers)
    {
      if (houseNumber.ToUpper().Contains(houseNumberFilter.ToUpper()))
      {
        HouseNumbersFiltered.Add(houseNumber);
      }
    }

    StatusInfo += " HouseNumbersFiltered.Count: " + HouseNumbersFiltered.Count;
  }

  /// <summary>
  /// Выбор номера дома из списка Popup.
  /// </summary>
  /// <param name="param"></param>
  private void SelectHouseNumber(object param)
  {
    HouseNumber = SelectedHouseNumber;
    ApplyFilterToHouseNumbers(param);
  }

  #endregion

  #region Private Properties

  private List<string> _citiesName = new List<string>();
  private List<string> _streetsName = new List<string>();
  private List<string> _houseNumbers = new List<string>();

  // private string _lastSelectCityName = string.Empty;
  
  #endregion

  #region Private Methods

  /// <summary>
  /// Первичное заполнение списка городов элементами.
  /// </summary>
  private void InitCitiesName()
  {
    if (_addressLocationViewModel.AddressLocations == null)
      throw new Exception("Err: " + nameof(InitCitiesName) + ". AddressLocations = null");

    foreach (var addressLocation in _addressLocationViewModel.AddressLocations)
    {
      if (addressLocation.CityName == null)
        throw new Exception("Err: " + nameof(InitCitiesName) + ". CityName = null");

      if (CitiesFiltered.Count == 0)
      {
        _citiesName.Add(addressLocation.CityName);
        CitiesFiltered.Add(addressLocation.CityName);
        continue;
      }

      var isNewCity = true;

      foreach (var cityName in _citiesName)
      {
        if (cityName == addressLocation.CityName)
        {
          isNewCity = false;
          break;
        }
      }

      if (isNewCity)
      {
        _citiesName.Add(addressLocation.CityName);
        CitiesFiltered.Add(addressLocation.CityName);
      }
    }
  }

  /// <summary>
  /// Первичное заполнение списка улиц. Список улиц зависит от выбранного города.
  /// </summary>
  private void InitStreetsName()
  {

    if (_addressLocationViewModel.AddressLocations == null)
      throw new Exception("Err: " + nameof(InitStreetsName) + ". AddressLocations = null");

    _streetsName.Clear();
    StreetsFiltered.Clear();

    StreetName = string.Empty;
    SelectedStreet = string.Empty;

    _houseNumbers.Clear();
    HouseNumbersFiltered.Clear();

    HouseNumber = string.Empty;
    SelectedHouseNumber = string.Empty;

    foreach (var addressLocation in _addressLocationViewModel.AddressLocations)
    {
      if (Equals(addressLocation.CityName, CityName))
      {
        if (addressLocation.StreetName == null)
          throw new Exception("Err: " + nameof(InitStreetsName) + ". StreetName = null");

        if (_streetsName.Count == 0)
        {
          _streetsName.Add(addressLocation.StreetName);
          StreetsFiltered.Add(addressLocation.StreetName);
          continue;
        }

        var isNewStreet = true;

        foreach (var streetName in _streetsName)
        {
          if (streetName == addressLocation.StreetName)
          {
            isNewStreet = false;
            break;
          }
        }

        if (isNewStreet)
        {
          _streetsName.Add(addressLocation.StreetName);
          StreetsFiltered.Add(addressLocation.StreetName);
        }
      }
    }
  }

  /// <summary>
  /// Первичное заполнение списка номеров домов. Зависит от выбранного города и улицы. 
  /// </summary>
  private void InitHouseNumbers()
  {
    if (_addressLocationViewModel.AddressLocations == null)
      throw new Exception("Err: " + nameof(InitHouseNumbers) + ". AddressLocations = null");

    _houseNumbers.Clear();
    HouseNumbersFiltered.Clear();

    HouseNumber = string.Empty;
    SelectedHouseNumber = string.Empty;

    foreach (var addressLocation in _addressLocationViewModel.AddressLocations)
    {
      if (Equals(addressLocation.CityName, CityName) && Equals(addressLocation.StreetName, StreetName))
      {
        if (addressLocation.HouseNumber == null)
          throw new Exception("Err: " + nameof(InitHouseNumbers) + ". HouseNumber = null");

        if (_houseNumbers.Count == 0)
        {
          _houseNumbers.Add(addressLocation.HouseNumber);
          HouseNumbersFiltered.Add(addressLocation.HouseNumber);
          continue;
        }

        var isNewHouseNumber = true;

        foreach (var houseNumber in _houseNumbers)
        {
          if (houseNumber == addressLocation.HouseNumber)
          {
            isNewHouseNumber = false;
            break;
          }
        }

        if (isNewHouseNumber)
        {
          _houseNumbers.Add(addressLocation.HouseNumber);
          HouseNumbersFiltered.Add(addressLocation.HouseNumber);
        }
      }
    }
  }

  #endregion

  #endregion

  #region Constructor

  public MainViewModel()
  {
    //ClientEr.CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
    //TODO: Хардкодим путь до папок с клиентами на время разработки и тестирования. В будущем значение будет вынесено в файл настроек "*.ini". 
    ClientEr.CurrentPath = "/mnt/share/Clients";

    OpenClient = new DelegateCommand(SelectClient);
    KeyUpClientName = new DelegateCommand(ApplyFilterToClientsList);

    KeyUpCityName = new DelegateCommand(ApplyFilterToCitiesName);
    LostFocusCityName = new DelegateCommand(FillStreetsName);
    TappedOnCityItem = new DelegateCommand(SelectCity);
    
    KeyUpStreetName = new DelegateCommand(ApplyFilterToStreetsName);
    LostFocusStreetName = new DelegateCommand(FillHouseNumbers);
    TappedOnStreetItem = new DelegateCommand(SelectStreet);

    KeyUpHouseNumber = new DelegateCommand(ApplyFilterToHouseNumbers);
    TappedOnHouseNumberItem = new DelegateCommand(SelectHouseNumber);
    
    _addressLocationViewModel = new AddressLocationViewModel();

    SortedClients = new ObservableCollection<ClientEntityViewModel>();

    SortedLocation = new ObservableCollection<string>();

    StatusInfo = ClientEr.CurrentPath;

    InitClientList();
    InitCitiesName();
  }

  #endregion

  #region Private Methods

  /// <summary>
  /// Проверяет и удаляет из имени по ссылке, запрещённые символы для именования папок в операцинной системе. 
  /// </summary>
  /// <param name="name">
  /// Ссылка на имя.
  /// </param>
  /// <returns>
  /// true - был найден запрещённый символ.
  /// false - запрещённых символов не обнаружено.
  /// </returns>
  private bool IsNameHaveInvalidCharacter(ref string name)
  {
    // Следующие зарезервированные символы:
    //  < - (меньше чем);
    //  > - (больше чем);
    //  : - (двоеточие)
    //  " - (двойная кавычка)
    //  / - (косая черта)
    //  \ - (обратная косая черта)
    //  | - (вертикальная полоса или канал)
    //  ? - (вопросительный знак)
    //  * - (звёздочка)

    if (name.Equals(string.Empty)) return false;

    switch (name.Last())
    {
      case '<':
      case '>':
      case ':':
      case '"':
      case '/':
      case '\\':
      case '|':
      case '?':
      case '*':
        if (name.Length != 1)
        {
          name = name.Substring(0, name.Length - 1);
        }
        else
        {
          name = String.Empty;
        }

        return true;
    }

    return false;
  }

  #endregion
}