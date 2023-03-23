using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows.Input;
using ClientExplorer.Application;

namespace ClientExplorer.Shared.ViewModels;

#pragma warning disable CS4014
public class MainViewModel : BaseViewModel
{
  #region For Windows

  #region Public Properties

  //Info панель
  public string StatusInfo { get; set; }
  public string LocationNameInfo { get; set; }

  public string VersionAppInfo { get; }

  #endregion

  #region Constructor

  public MainViewModel()
  {
    LocationNameInfo = string.Empty;
    StatusInfo = "Run";
    VersionAppInfo = ClientExplorerApp.VersionApp;

    //TODO Вынести в настройки CurrentPath (расположение папок клиентов)
    //Пишем в коде, путь до папок с клиентами на время разработки и тестирования.
    //В будущем значение будет вынесено в файл настроек "*.ini".
    //Или будем использовать путь расположения программы, т.к. программа рассчитана и на Windows и на Linux.
    // ClientEr.CurrentPath = "../";
    //ClientEr.CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
    ClientExplorerApp.CurrentPath = "/mnt/share/Clients";

    // var setting = new ClientExplorerSetting();
    //
    // var options = new JsonSerializerOptions
    // {
    //   Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
    //   WriteIndented = true
    // };
    //
    // if (!Directory.Exists("./Assets")) Directory.CreateDirectory("./Assets");
    // using var fs = new FileStream("./Assets/Setting.json", FileMode.Create, FileAccess.Write);
    // JsonSerializer.Serialize(fs, setting, options);

    // LocationNameInfo = AppDomain.CurrentDomain.BaseDirectory;

    #region Client

    TappedClientItem = new DelegateCommand(SelectClient);
    KeyUpClientName = new DelegateCommand(KeyUpInClientName);
    LostFocusClientName = new DelegateCommand(LostFocusInClientName);

    InitClientListAsync();

    #endregion

    #region Location of Client

    TappedOnLocationClientItem = new DelegateCommand(SelectLocation);

    #endregion

    #region Address Location

    _addressLocationViewModel = new AddressLocationViewModel();

    KeyUpCityName = new DelegateCommand(ApplyFilterToCitiesName);
    LostFocusCityName = new DelegateCommand(FillStreetsName);
    TappedOnCityItem = new DelegateCommand(SelectCity);

    KeyUpStreetName = new DelegateCommand(ApplyFilterToStreetsName);
    LostFocusStreetName = new DelegateCommand(FillHouseNumbers);
    TappedOnStreetItem = new DelegateCommand(SelectStreet);

    KeyUpHouseNumber = new DelegateCommand(ApplyFilterToHouseNumbers);
    TappedOnHouseNumberItem = new DelegateCommand(SelectHouseNumber);

    KeyUpAdditionalInfo = new DelegateCommand(KeyUpInAdditionalInfo);

    InitCitiesName();

    #endregion

    #region Folders

    KeyUpFolderNameUserVersion = new DelegateCommand(KeyUpInFolderNameUserVersion);

    #endregion

    foreach (var directory in ClientExplorerApp.DirectoriesInLocation.Folders)
    {
      FoldersForCreate.Add(new FolderLocationEntityViewModel(directory.Name, directory));
    }
    
    var currDir = ClientExplorerApp.CurrentPath + Path.DirectorySeparatorChar + ClientExplorerApp.DefaultDataResourcePath;
    
    if (Directory.Exists(currDir))
    {
      var msg = "Ready";
      ShowMessageInfoAsync(msg, 0.0, false);
    }
    else
    {
      var msg = currDir + " - not available...";
      ShowMessageInfoAsync(msg, 0.0, false);
    }
  }

  #endregion

  #region Private Methods

  /// <summary>
  /// Проверяет/удаляет из имени по ссылке, запрещённые символы для имени папки в операционной системе. 
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
    /*
    Следующие зарезервированные символы:
      Системные:
        < - (меньше чем);
        > - (больше чем);
        : - (двоеточие)
        " - (двойная кавычка)
        / - (косая черта)
        \ - (обратная косая черта)
        | - (вертикальная полоса или канал)
        ? - (вопросительный знак)
        * - (звёздочка)
      Только для приложения:        
        ' - (апостроф) символ используется для выделения поля 'дополнительная информация'  
        , - (запятая) символ используется для разделения (город, улица, номер дома)
    */

    if (name.Equals(string.Empty)) return false;

    switch (name.Last())
    {
      case ' ':
        // не допускаем длинные пробелы
        name = name.Length switch
        {
          1 => string.Empty,
          > 2 when name[^2] == ' ' => name.Substring(0, name.Length - 1),
          _ => name
        };
        return true;
      case '<':
      case '>':
      case ':':
      case '"':
      case '/':
      case '\\':
      case '|':
      case '?':
      case '*':
      case '\'':
      case ',':
        name = name.Length != 1 ? name.Substring(0, name.Length - 1) : string.Empty;
        return true;
    }

    return false;
  }

  /// <summary>
  /// Вывод сообщений в поле "информация" асинхронный.
  /// </summary>
  /// <param name="msg">
  ///   Сообщение
  /// </param>
  /// <param name="timeShowInSec">
  ///   Время показа сообщений в секундах, по умолчанию полсекунды
  /// </param>
  /// <param name="eraseAfter">
  ///   Очистить поле после показа? По умолчанию 'true' - очистить
  /// </param>
  /// <returns>
  /// Отметка о завершении.
  /// </returns>
  private async Task ShowMessageInfoAsync(string msg, double timeShowInSec = 2.0, bool eraseAfter = true)
  {
    StatusInfo = msg;
    
    if (eraseAfter)
    {
      await Task.Delay((int)(1000 * timeShowInSec));
      StatusInfo = string.Empty;
    }
  }

  #endregion

  #endregion

  //Клиент

  #region Client

  #region Public Properties

  public string ClientFilter { get; set; } = string.Empty;

  public ObservableCollection<ClientEntityViewModel> SortedClients { get; set; } =
    new ObservableCollection<ClientEntityViewModel>();

  public ClientEntityViewModel? SelectedClient { get; set; }

  // public bool IsInitClient { get; private set; } = false;

  #endregion

  #region Public Methods

  public void OnClickButtonCancelClient()
  {
    Task.Run(InitClientListAsync);

    ClientFilter = string.Empty;

    SelectedLocation = string.Empty;
    FoldersForCreateDefault();
    SortedLocationsOfClient.Clear();

    IsLocationOfClientEmpty = SortedLocationsOfClient.Count <= 0;
  }

  #endregion

  #region Events

  public ICommand TappedClientItem { get; }
  public ICommand KeyUpClientName { get; }

  public ICommand LostFocusClientName { get; }

  #endregion

  #region Commands Methods

  /// <summary>
  /// Выделение клиента.
  /// </summary>
  /// <param name="param">
  /// ClientEntityViewModel - выделенный клиент
  /// </param>
  private void SelectClient(object param)
  {
    if (SelectedClient == null)
    {
      var msg = string.Format(
        "Err: In methods - {0}. {1} == null",
        nameof(SelectClient),
        nameof(SelectedClient));
      ShowMessageInfoAsync(msg);
      return;
    }

    ClientFilter = SelectedClient.Name;

    var selected = SelectedClient;
    ApplyFilterToClientsList();

    SelectedClient = SortedClients.Count == 1 ? SortedClients[0] : selected;

    LoadLocationsOfClient();

    if (string.IsNullOrEmpty(CityName) || string.IsNullOrEmpty(AdditionalInfo))
    {
      ApplyFilterToLocationOfClient();
    }
  }

  private void KeyUpInClientName(object param)
  {
    //TODO Проверить нужно ли очищать SelectedClient
    SelectedClient = null;
    ApplyFilterToClientsList();

    LostFocusInClientName(param);

    AdditionalInfo = string.Empty;
    CityName = string.Empty;
    StreetName = string.Empty;
    HouseNumber = string.Empty;
    IsSelectedLocation = false;
    IsLocationAvailable = false;
    LocationNameInfo = GetLocationName();

    FoldersForCreateDefault();
  }

  private void LostFocusInClientName(object param)
  {
    foreach (var client in SortedClients)
    {
      if (client.Name.Equals(ClientFilter))
      {
        SelectedClient = client;
        break;
      }
    }

    LoadLocationsOfClient();
  }

  /// <summary>
  /// Применение фильтра к 'Списку Клиентов' (SortedClients). На основании (ClientFilter).
  /// </summary>
  private void ApplyFilterToClientsList()
  {
    // Сброс листа объектов клиента.
    SortedLocationsOfClient.Clear();

    IsLocationOfClientEmpty = SortedLocationsOfClient.Count <= 0;

    var clientFilter = ClientFilter;

    var msg = "ClientFilter: " + clientFilter;
    ShowMessageInfoAsync(msg);

    // Пустой фильтр
    if (Equals(clientFilter, string.Empty))
    {
      SortedClients.Clear();

      foreach (var client in _clientsList)
      {
        SortedClients.Add(client);
      }

      msg += " SortedClients.Count: " + SortedClients.Count;
      ShowMessageInfoAsync(msg);

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
      if (client.Name.ToUpper().Contains(clientFilter.ToUpper()))
      {
        SortedClients.Add(client);
      }
    }

    // Выделить клиента если он остался один в списке...
    if (SortedClients.Count == 1 && SortedClients[0].Name == clientFilter)
    {
      SelectedClient = SortedClients[0];
      LoadLocationsOfClient();

      ApplyFilterToLocationOfClient();
    }

    msg += " SortedClients.Count: " + SortedClients.Count;
    ShowMessageInfoAsync(msg);
  }

  #endregion

  #region Private Properties

  private List<ClientEntityViewModel> _clientsList = new List<ClientEntityViewModel>();

  #endregion

  #region Private Methods

  /// <summary>
  /// Первичное заполнение списка клиентов
  /// </summary>
  private Task InitClientListAsync()
  {
    if (ClientExplorerApp.CurrentPath == null)
    {
      var msg = string.Format("Err: In methods - {0}. {1} == null", nameof(InitClientListAsync),
        nameof(ClientExplorerApp.CurrentPath));
      ShowMessageInfoAsync(msg, 0.0, false);
      return Task.CompletedTask;
    }

    SortedClients.Clear();

    var directoryInfo = new DirectoryInfo(ClientExplorerApp.CurrentPath);

    if (!Directory.Exists(ClientExplorerApp.CurrentPath))
    {
      var msg = string.Format("Err: In methods - {0}. {1} - not available...", nameof(InitClientListAsync),
        nameof(ClientExplorerApp.CurrentPath));
      ShowMessageInfoAsync(msg, 0.0, false);
      return Task.CompletedTask;
    }

    foreach (var directory in directoryInfo.GetDirectories())
    {
      //  Точка вначале - символ скрытности. Такие директории пропускать. 
      if (directory.Name.First() != '.')
      {
        SortedClients.Add(new ClientEntityViewModel(directory.Name, new DirectoryInfo(directory.FullName)));
      }
    }

    SortedClients = new ObservableCollection<ClientEntityViewModel>(SortedClients.OrderBy(i => i.Name));

    _clientsList = SortedClients.ToList();
    return Task.CompletedTask;
  }
  
  #endregion

  #endregion

  // Адреса объектов (локации)

  #region Address Location

  #region Public Properties

  public bool IsSelectedLocation { get; private set; }

  public ObservableCollection<string> CitiesFiltered { get; set; } =
    new ObservableCollection<string>();

  public string CityName { get; set; } = string.Empty;
  public string SelectedCity { get; set; } = string.Empty;

  public ObservableCollection<string> StreetsFiltered { get; set; } =
    new ObservableCollection<string>();

  public string StreetName { get; set; } = string.Empty;
  public string SelectedStreet { get; set; } = string.Empty;

  public ObservableCollection<string> HouseNumbersFiltered { get; set; } =
    new ObservableCollection<string>();

  public string HouseNumber { get; set; } = string.Empty;
  public string SelectedHouseNumber { get; set; } = string.Empty;

  public string AdditionalInfo { get; set; } = string.Empty;

  public bool IsLocationAvailable { get; set; } 

  #endregion

  #region Public Methods

  public void OnClickButtonClearCityName()
  {
    // Очистить Название Города
    // Отменить фильтр списка (города)
    // Очистить Название улицы, номер дома.
    // Очистить списки улиц, списки номеров домов
    OnClickButtonClearStreetName();
    CityName = SelectedCity = string.Empty;
    ApplyFilterToCitiesName(CityName);

    LocationNameInfo = GetLocationName();
  }

  public void OnClickButtonClearStreetName()
  {
    // Очистить Название улицы
    // Отменить фильтр с названия улиц
    // Очистить номер дома
    // Очистить список номеров домов
    OnClickButtonClearHouseNumber();
    StreetName = SelectedStreet = string.Empty;
    InitStreetsName();

    LocationNameInfo = GetLocationName();
  }

  public void OnClickButtonClearHouseNumber()
  {
    // Очистить номер дома
    // Применить фильтр по списку номеров домов
    HouseNumber = SelectedHouseNumber = string.Empty;
    InitHouseNumbers();

    LocationNameInfo = GetLocationName();
  }

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

  public ICommand KeyUpAdditionalInfo { get; }

  #endregion

  #region Commands Methods

  /// <summary>
  /// Применение фильтра к 'Списку Населённых пунктов' (CitiesFiltered). На основании (CityName).
  /// </summary>
  /// <param name="param"></param>
  private void ApplyFilterToCitiesName(object param)
  {
    var cityFilter = CityName;

    var msg = "CityFilter: " + cityFilter;
    ShowMessageInfoAsync(msg);

    if (Equals(cityFilter, string.Empty))
    {
      CitiesFiltered.Clear();

      foreach (var cityName in _citiesName)
      {
        CitiesFiltered.Add(cityName);
      }

      msg += " CitiesFiltered.Count: " + CitiesFiltered.Count;
      ShowMessageInfoAsync(msg);

      ApplyFilterToLocationOfClient();
      return;
    }

    if (IsNameHaveInvalidCharacter(ref cityFilter))
    {
      CityName = cityFilter;
      ChangeIsLocationAvailable();
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

    ChangeIsLocationAvailable();

    ApplyFilterToLocationOfClient();
    msg += " CitiesFiltered.Count: " + CitiesFiltered.Count;
    ShowMessageInfoAsync(msg);

    LocationNameInfo = GetLocationName();
  }

  private void ChangeIsLocationAvailable()
  {
    IsLocationAvailable = !string.IsNullOrEmpty(CityName) || !string.IsNullOrEmpty(AdditionalInfo);
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
    ApplyFilterToLocationOfClient();

    LocationNameInfo = GetLocationName();
  }

  /// <summary>
  /// Заполнение списка улиц
  /// </summary>
  /// <param name="param"></param>
  private void FillStreetsName(object param)
  {
    InitStreetsName();
    LocationNameInfo = GetLocationName();
  }

  /// <summary>
  /// Применение фильтра к 'Списку Улиц' (StreetsFiltered). На основании (CityName).
  /// </summary>
  /// <param name="param"></param>
  private void ApplyFilterToStreetsName(object? param)
  {
    var streetFilter = StreetName;

    var msg = "StreetsFiltered: " + streetFilter;
    ShowMessageInfoAsync(msg);

    if (Equals(streetFilter, string.Empty))
    {
      StreetsFiltered.Clear();

      foreach (var streetName in _streetsName)
      {
        StreetsFiltered.Add(streetName);
      }

      msg += " StreetsFiltered.Count: " + StreetsFiltered.Count;
      ShowMessageInfoAsync(msg);

      ApplyFilterToLocationOfClient();
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

    ApplyFilterToLocationOfClient();

    msg += " StreetsFiltered.Count: " + StreetsFiltered.Count;
    ShowMessageInfoAsync(msg);

    LocationNameInfo = GetLocationName();
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
    ApplyFilterToLocationOfClient();

    LocationNameInfo = GetLocationName();
  }

  /// <summary>
  /// Заполнение списка номеров домов
  /// </summary>
  /// <param name="param"></param>
  private void FillHouseNumbers(object param)
  {
    InitHouseNumbers();

    LocationNameInfo = GetLocationName();
  }

  /// <summary>
  /// Применение фильтра к 'Списку номеров домов' (HouseNumbersFiltered). На основании (HouseNumber).
  /// </summary>
  /// <param name="param"></param>
  private void ApplyFilterToHouseNumbers(object? param)
  {
    var houseNumberFilter = HouseNumber;

    var msg = "HouseNumberFilter: " + houseNumberFilter;
    ShowMessageInfoAsync(msg);

    if (Equals(houseNumberFilter, string.Empty))
    {
      HouseNumbersFiltered.Clear();

      foreach (var houseNumber in _houseNumbers)
      {
        HouseNumbersFiltered.Add(houseNumber);
      }

      msg += " HouseNumbersFiltered.Count: " + HouseNumbersFiltered.Count;
      ShowMessageInfoAsync(msg);

      ApplyFilterToLocationOfClient();
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

    ApplyFilterToLocationOfClient();

    msg += " HouseNumbersFiltered.Count: " + HouseNumbersFiltered.Count;
    ShowMessageInfoAsync(msg);

    LocationNameInfo = GetLocationName();
  }

  /// <summary>
  /// Выбор номера дома из списка Popup.
  /// </summary>
  /// <param name="param"></param>
  private void SelectHouseNumber(object param)
  {
    HouseNumber = SelectedHouseNumber;
    ApplyFilterToHouseNumbers(param);
    ApplyFilterToLocationOfClient();

    LocationNameInfo = GetLocationName();
  }

  /// <summary>
  /// Ввод текста в поле 'Дополнительная информация'
  /// </summary>
  /// <param name="param">
  /// Значение поля
  /// </param>
  private void KeyUpInAdditionalInfo(object param)
  {
    var additionalInfo = AdditionalInfo;

    if (IsNameHaveInvalidCharacter(ref additionalInfo))
    {
      AdditionalInfo = additionalInfo;
      ChangeIsLocationAvailable();
      return;
    }

    ApplyFilterToLocationOfClient();
    ChangeIsLocationAvailable();

    LocationNameInfo = GetLocationName();
  }

  /// <summary>
  /// Команда. Нажатие на кнопку сброса выделенного объекта (локации).
  /// </summary>
  public void OnClickButtonClearAdditionalInfo()
  {
    if (IsSelectedLocation)
    {
      CityName = string.Empty;
      StreetName = string.Empty;
      HouseNumber = string.Empty;

      ApplyFilterToCitiesName(CityName);
    }

    IsSelectedLocation = false;
    SelectedLocation = string.Empty;
    FoldersForCreateDefault();
    AdditionalInfo = string.Empty;
    ApplyFilterToLocationOfClient();
    ChangeIsLocationAvailable();

    LocationNameInfo = GetLocationName();
  }

  #endregion

  #region Private Properties

  private readonly AddressLocationViewModel _addressLocationViewModel;

  private readonly List<string> _citiesName = new List<string>();
  private readonly List<string> _streetsName = new List<string>();
  private readonly List<string> _houseNumbers = new List<string>();
  
  #endregion

  #region Private Methods

  /// <summary>
  /// Первичное заполнение списка городов элементами.
  /// </summary>
  private void InitCitiesName()
  {

    foreach (var addressLocation in _addressLocationViewModel.AddressLocations)
    {
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

    _houseNumbers.Clear();
    HouseNumbersFiltered.Clear();

    HouseNumber = string.Empty;
    SelectedHouseNumber = string.Empty;

    foreach (var addressLocation in _addressLocationViewModel.AddressLocations)
    {
      if (Equals(addressLocation.CityName, CityName) && Equals(addressLocation.StreetName, StreetName))
      {
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

  //Существующие объекты клиента

  #region Location of Client

  #region Public Properties

  public ObservableCollection<string> SortedLocationsOfClient { get; set; } = new ObservableCollection<string>();
  public string SelectedLocation { get; set; } = string.Empty;

  public bool IsLocationOfClientEmpty { get; set; } = true;

  #endregion

  #region Events

  public ICommand TappedOnLocationClientItem { get; }

  #endregion

  #region Command Methods

  /// <summary>
  /// Команда. Выделение объекта (локации)
  /// </summary>
  /// <param name="param">
  /// Выделенный объект (локация)
  /// </param>
  private void SelectLocation(object param)
  {
    if (param.ToString() == string.Empty) return;
    AdditionalInfo = SelectedLocation;
    CityName = string.Empty;
    StreetName = string.Empty;
    HouseNumber = string.Empty;
    ApplyFilterToLocationOfClient();

    foreach (var locationOfClient in SortedLocationsOfClient)
    {
      if (locationOfClient.Equals(AdditionalInfo))
      {
        SelectedLocation = locationOfClient;
        break;
      }
    }

    ChangeIsLocationAvailable();
    IsSelectedLocation = true;

    LocationNameInfo = GetLocationName();

    CheckLocationForFolders();
  }

  #endregion

  #region Private Properties

  private List<string> LocationsOfClient { get; set; } = new List<string>();

  #endregion

  #region Private Methods

  /// <summary>
  /// Загружает существующие объекты у клиента из директории 'Объекты'
  /// </summary>
  private void LoadLocationsOfClient()
  {
    SortedLocationsOfClient.Clear();
    LocationsOfClient.Clear();

    if (SelectedClient == null)
    {
      var msg = string.Format("Err: In methods - {0}. {1} == null", nameof(LoadLocationsOfClient),
        nameof(SelectedClient));
      ShowMessageInfoAsync(msg);
      return;
    }

    var directoryPath = SelectedClient.ClientPath.FullName + Path.DirectorySeparatorChar +
                        ClientExplorerApp.FolderObjectsName;

    if (!Directory.Exists(directoryPath))
    {
      var msg = "Err: '" + directoryPath + "' не обнаружены";
      ShowMessageInfoAsync(msg);
      return;
    }

    var directoryInfo = new DirectoryInfo(directoryPath);

    foreach (var directory in directoryInfo.GetDirectories())
    {
      SortedLocationsOfClient.Add(directory.Name);
    }

    SortedLocationsOfClient = new ObservableCollection<string>(SortedLocationsOfClient.OrderBy(i => i));
    LocationsOfClient = new List<string>(SortedLocationsOfClient.ToList());

    IsLocationOfClientEmpty = SortedLocationsOfClient.Count <= 0;
  }


  /// <summary>
  /// Фильтрация списка 'Объектов (локаций)' клиента
  /// </summary>
  /// <param name="source">
  /// Копия оригинального списка
  /// </param>
  /// <param name="filter">
  /// Основание для фильтрации
  /// </param>
  private void FilteringList(ref List<string> source, string filter)
  {
    List<string> buffer = new List<string>();
    foreach (var item in source)
    {
      if (item.ToUpper().Contains(filter.ToUpper()))
      {
        buffer.Add(item);
      }
    }

    source = buffer;
  }

  /// <summary>
  /// Применение фильтра к списку 'Объектов (локаций)' клиента
  /// </summary>
  private void ApplyFilterToLocationOfClient()
  {
    if (LocationsOfClient.Count > 0)
    {
      List<string> buffer = new List<string>(LocationsOfClient);

      SortedLocationsOfClient.Clear();

      if (buffer.Count > 0 && CityName != string.Empty) FilteringList(ref buffer, CityName);
      if (buffer.Count > 0 && StreetName != string.Empty) FilteringList(ref buffer, StreetName);
      if (buffer.Count > 0 && HouseNumber != string.Empty) FilteringList(ref buffer, HouseNumber);
      if (buffer.Count > 0 && AdditionalInfo != string.Empty) FilteringList(ref buffer, AdditionalInfo);

      if (buffer.Count > 0)
      {
        foreach (var item in buffer)
        {
          SortedLocationsOfClient.Add(item);
        }
      }
    }

    IsLocationOfClientEmpty = SortedLocationsOfClient.Count <= 0;
  }

  #endregion

  #endregion

  //Папки

  #region Folders

  #region Public Properties

  public ObservableCollection<FolderLocationEntityViewModel> FoldersForCreate { get; set; } =
    new ObservableCollection<FolderLocationEntityViewModel>();

  public bool FolderNameUserVersionIsCheck { get; set; }
  public string FolderNameUserVersion { get; set; } = string.Empty;

  #endregion

  #region Event

  public ICommand KeyUpFolderNameUserVersion { get; }

  #endregion

  #region Command Methods

  private void KeyUpInFolderNameUserVersion(object param)
  {
    var folderNameUserVersion = FolderNameUserVersion;

    if (IsNameHaveInvalidCharacter(ref folderNameUserVersion))
    {
      FolderNameUserVersion = folderNameUserVersion;
      return;
    }

    FolderNameUserVersionIsCheck = FolderNameUserVersion.Length > 0;
  }

  #endregion

  /// <summary>
  /// Проверить активированную локацию клиента на наличие стандартных папок
  /// Ошибка: Нет выделенного клиента. Ничего не сделает.
  /// </summary>
  private void CheckLocationForFolders()
  {
    if (SelectedClient == null)
    {
      var msg = string.Format("Err: In methods - {0}. {1} == null", nameof(CheckLocationForFolders),
        nameof(SelectedClient));
      ShowMessageInfoAsync(msg);
      return;
    }

    var pathForObject = SelectedClient.ClientPath.FullName + Path.DirectorySeparatorChar +
                        ClientExplorerApp.FolderObjectsName;

    foreach (var folderForCreate in FoldersForCreate)
    {
      folderForCreate.IsCheck = false;
      folderForCreate.IsEnable = true;

      var dir = pathForObject + Path.DirectorySeparatorChar + SelectedLocation +
                folderForCreate.FolderDirectory.GetDirectoryPath();

      if (Directory.Exists(dir))
      {
        folderForCreate.IsCheck = true;
        folderForCreate.IsEnable = false;
      }
    }
  }

  /// <summary>
  /// Установка полей IsCheck, IsEnable в состояние по-умолчанию
  /// </summary>
  private void FoldersForCreateDefault()
  {
    foreach (var folderForCreate in FoldersForCreate)
    {
      folderForCreate.IsCheck = false;
      folderForCreate.IsEnable = true;
    }
  }

  #endregion

  // Кнопки

  #region Buttons

  #region Commands Methods

  /// <summary>
  /// Команда. Запуск алгоритма создания директорий
  /// </summary>
  public async Task<Task> OnClickButtonCreateDirectory()
  {
    string clientPath;

    if (SelectedClient == null)
    {
      clientPath = ClientExplorerApp.CurrentPath + Path.DirectorySeparatorChar + ClientFilter;

      // Создаёт папку клиента. Если это новый клиент.
      if (!Directory.Exists(clientPath))
      {
        Directory.CreateDirectory(clientPath);

        await InitClientListAsync();

        ApplyFilterToClientsList();
      }
    }
    else
    {
      clientPath = SelectedClient.ClientPath.FullName;
    }

    // Выделяем клиента.
    foreach (var client in SortedClients)
    {
      if (client.Name.Equals(ClientFilter))
      {
        SelectedClient = client;
        break;
      }
    }

    // Создаёт в корневой папке клиента стандартный набор директорий.
    foreach (var directoryInClient in ClientExplorerApp.DirectoriesInClient.Folders)
    {
      await CreateFolder(clientPath, directoryInClient);
    }

    // Составляет путь директории объекта (локации)
    var locationPath = clientPath + Path.DirectorySeparatorChar + ClientExplorerApp.FolderObjectsName;
    var isValidLocation = false;

    locationPath = GetLocationPath(locationPath, ref isValidLocation);

    if (isValidLocation)
    {
      // Создаёт папку объекта. Если это новый объект.
      if (!Directory.Exists(locationPath))
      {
        Directory.CreateDirectory(locationPath);

        LoadLocationsOfClient();
      }

      var locationName = GetLocationName();

      foreach (var locationOfClient in SortedLocationsOfClient)
      {
        if (Equals(locationOfClient, locationName))
        {
          SelectedLocation = locationOfClient;
          break;
        }
      }

      IsSelectedLocation = true;

      var msg = locationPath;
      await ShowMessageInfoAsync(msg, 0.25);

      // Создаёт в папке объекта (локации) отмеченные директории из стандартного набора.
      foreach (var folderForCreate in FoldersForCreate)
      {
        if (folderForCreate.IsCheck)
        {
          await CreateFolder(locationPath, folderForCreate.FolderDirectory);
        }
      }

      if (FolderNameUserVersionIsCheck)
      {
        await CreateFolder(locationPath, new DirectoryEntity(FolderNameUserVersion));

        FolderNameUserVersion = string.Empty;
        FolderNameUserVersionIsCheck = false;
      }
      
      var buf = SelectedLocation;

      ApplyFilterToLocationOfClient();

      SelectedLocation = buf;

      CheckLocationForFolders();
      
      msg = "Info: Create completed - Ok";
      await ShowMessageInfoAsync(msg);
    }
    
    return Task.CompletedTask;
  }

  #endregion

  #region Private Methods

  /// <summary>
  /// Получить имя объекта (локации)
  /// </summary>
  /// <returns>
  /// Имя объекта (локации)
  /// </returns>
  private string GetLocationName()
  {
    // Формирование имени локации.
    var name = string.Empty;

    if (CityName != string.Empty)
    {
      name += CityName;
    }

    if (StreetName != string.Empty)
    {
      name += ", " + StreetName;
    }

    if (HouseNumber != string.Empty)
    {
      name += ", " + HouseNumber;
    }

    if (AdditionalInfo != string.Empty)
    {
      if (CityName != string.Empty)
      {
        name += " - ";
      }

      name += AdditionalInfo;
    }

    return name;
  }

  /// <summary>
  /// Формирование полного пути до создаваемого объекта (локации)
  /// </summary>
  /// <param name="startPath">
  /// Начальный путь, относительно которого будет сформирован остальной путь.
  /// </param>
  /// <param name="isValidLocation">
  /// Выполнение требований по минимальной информации, которую необходимо заполнить.
  /// true - выполнено, путь составлен
  /// false - не выполнен, вернётся стартовый путь...
  /// </param>
  /// <returns>
  /// Полный путь до папки объекта (локации)
  /// </returns>
  private string GetLocationPath(string startPath, ref bool isValidLocation)
  {
    if (CityName != string.Empty)
    {
      isValidLocation = true;

      // Сохраняет адрес объекта (локации), если его ещё нет в базе.
      if (_addressLocationViewModel.SaveLocationIfNew(CityName, StreetName, HouseNumber))
      {
        // Перечитываем список городов
        InitCitiesName();
      }
    }

    if (AdditionalInfo != string.Empty)
    {
      isValidLocation = true;
    }

    InitCitiesName();
    ApplyFilterToCitiesName(CityName);

    AdditionalInfo = GetLocationName();

    CityName = string.Empty;
    StreetName = string.Empty;
    HouseNumber = string.Empty;

    return startPath + Path.DirectorySeparatorChar + AdditionalInfo;
  }

  /// <summary>
  /// Создаёт дерево директорий относительно указанного пути.
  /// Метод проходит по дереву директорий рекурсивно. Ныряет вглубь.
  /// </summary>
  /// <param name="locationPath">
  /// Путь, в котором будет происходить создание директории
  /// </param>
  /// <param name="directoryEntity">
  /// Путь, который необходимо создать.
  /// </param>
  private async Task CreateFolder(string locationPath, DirectoryEntity directoryEntity)
  {
    var dir = locationPath + directoryEntity.GetDirectoryPath();

    if (!Directory.Exists(dir))
    {
      Directory.CreateDirectory(dir);
      
      var msg = directoryEntity.Name + " +";
      await ShowMessageInfoAsync(msg, 0.10);
    }

    if (directoryEntity.ChildDirs != null)
    {
      foreach (var childDir in directoryEntity.ChildDirs)
      {
        await CreateFolder(locationPath, childDir);
      }
    }
  }

  #endregion

  #endregion
}