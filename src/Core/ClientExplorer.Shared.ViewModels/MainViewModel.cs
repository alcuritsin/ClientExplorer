using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using ClientExplorer.Application;

namespace ClientExplorer.Shared.ViewModels;

public class MainViewModel : BaseViewModel
{
  #region For Windows

  #region Public Properties

  //Info панель
  public string StatusInfo { get; set; }

  #endregion

  #region Constructor

  public MainViewModel()
  {
    //ClientEr.CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
    //TODO: Вынести в настройки CurrentPath (расположение папок клиентов)
    //Хардкодим путь до папок с клиентами на время разработки и тестирования.
    //В будущем значение будет вынесено в файл настроек "*.ini".
    //Или будем использовать путь расположения программы, т.к. программа рассчитана и на Windows и на Linux.
    ClientEr.CurrentPath = "/mnt/share/Clients";

    #region Client

    OpenClient = new DelegateCommand(SelectClient);
    KeyUpClientName = new DelegateCommand(ApplyFilterToClientsList);

    InitClientList();

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

    KeyUpAdditionalParam = new DelegateCommand(EnterAdditionalParam);

    KeyUpFolderNameUserVersion = new DelegateCommand(EnterFolderNameUserVersion);

    InitCitiesName();

    #endregion


    foreach (var directory in ClientEr.DirectoriesInLocation.Folders)
    {
      FoldersForCreate.Add(new FolderLocationEntityViewModel(directory.Name, directory));
    }


    //TODO Publication. Debug
    var currDir = ClientEr.CurrentPath + Path.DirectorySeparatorChar + ClientEr.DefaultDataResourcePath;
    if (Directory.Exists(currDir))
    {
      StatusInfo = "Ready";
    }
    else
    {
      StatusInfo = currDir + " - not available...";
    }
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
    /*
    Следующие зарезервированные символы:
     < - (меньше чем);
     > - (больше чем);
     : - (двоеточие)
     " - (двойная кавычка)
     / - (косая черта)
     \ - (обратная косая черта)
     | - (вертикальная полоса или канал)
     ? - (вопросительный знак)
     * - (звёздочка)
     */

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

  #region Events

  public ICommand OpenClient { get; }
  public ICommand KeyUpClientName { get; }

  #endregion

  #region Commands Methods

  private void SelectClient(object param)
  {
    if (SelectedClient.Name != null) ClientFilter = SelectedClient.Name;
    

    ClientFilter = SelectedClient.Name;
    
    ApplyFilterToClientsList(ClientFilter);

    SelectedClient = SortedClients[0];
    
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
    // Сброс листа объектов клиента.
    SortedLocationsOfClient.Clear();

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
        throw new Exception("Err: " + nameof(ApplyFilterToClientsList) + ". " + nameof(client.Name) +
                            " = null");

      if (client.Name.ToUpper().Contains(clientFilter.ToUpper()))
      {
        SortedClients.Add(client);
      }
    }

    //TODO Выделить клиента если он остался один в списке... 

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
      //  Точка вначале - символ скрытности. Такие директории пропускать. 
      if (directory.Name.First() != '.')
      {
        SortedClients.Add(new ClientEntityViewModel(directory.Name, new DirectoryInfo(directory.FullName)));
      }
    }

    SortedClients = new ObservableCollection<ClientEntityViewModel>(SortedClients.OrderBy(i => i.Name));

    _clientsList = SortedClients.ToList();
  }

  /// <summary>
  /// Проверяет директорию клиента на наличие стандартных папок
  /// </summary>
  /// <returns>
  /// true - Директория клиента содержит все необходимые папки
  /// false - В директории клиента не хватает как минимум одной папки стандартной папки
  /// </returns>
  private bool CheckClientToInit()
  {
    //TODO Publication.Временный метод для тестирования. Удалить перед публикацией.
    foreach (DirectoryEntity folder in ClientEr.DirectoriesInClient.Folders)
    {
      var clientDirectory = SelectedClient.ClientPath.FullName + Path.DirectorySeparatorChar + folder.Name;

      if (!Directory.Exists(clientDirectory)) return false;
    }

    return true;
  }

  #endregion

  #endregion

  //Существующие объекты клиента

  #region Location of Client

  #region Public Properties

  public ObservableCollection<string> SortedLocationsOfClient { get; set; } = new ObservableCollection<string>();
  public string SelectedLocation { get; set; } = string.Empty;

  #endregion

  #region Events

  public ICommand TappedOnLocationClientItem { get; }

  #endregion

  #region Command Methods

  private void SelectLocation(object param)
  {
    AdditionalParam = SelectedLocation;
    ApplyFilterToLocationOfClient();
    SelectedLocation = SortedLocationsOfClient[0];
    CheckLocationForFolders();
  }

  #endregion

  #region Private Properties

  private List<string> _locationsOfClient { get; set; } = new List<string>();

  #endregion

  #region Private Methods

  /// <summary>
  /// Загружает существующие объекты у клиента из директории 'Объекты'
  /// </summary>
  private void LoadClientLocation()
  {
    SortedLocationsOfClient.Clear();
    _locationsOfClient.Clear();

    if (SelectedClient.ClientPath == null)
    {
      StatusInfo = "Err: SelectedClient.ClientPath == null";
      return;
    }

    var directoryPath = SelectedClient.ClientPath.FullName + Path.DirectorySeparatorChar +
                        ClientEr.FolderObjectsName;

    if (!Directory.Exists(directoryPath))
    {
      StatusInfo = "Err: '" + directoryPath + "' не обнаружены";
      return;
    }

    var directoryInfo = new DirectoryInfo(directoryPath);

    foreach (var directory in directoryInfo.GetDirectories())
    {
      SortedLocationsOfClient.Add(directory.Name);
    }

    SortedLocationsOfClient = new ObservableCollection<string>(SortedLocationsOfClient.OrderBy(i => i));
    _locationsOfClient = new List<string>(SortedLocationsOfClient.ToList());
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
    if (_locationsOfClient.Count > 0)
    {
      List<string> buffer = new List<string>(_locationsOfClient);

      SortedLocationsOfClient.Clear();

      if (buffer.Count > 0 && CityName != string.Empty) FilteringList(ref buffer, CityName);
      if (buffer.Count > 0 && StreetName != string.Empty) FilteringList(ref buffer, StreetName);
      if (buffer.Count > 0 && HouseNumber != string.Empty) FilteringList(ref buffer, HouseNumber);
      if (buffer.Count > 0 && AdditionalParam != string.Empty) FilteringList(ref buffer, AdditionalParam);

      if (buffer.Count > 0)
      {
        foreach (var item in buffer)
        {
          SortedLocationsOfClient.Add(item);
        }
      }
    }
  }

  #endregion

  #endregion

  // Адреса объектов (локации)

  #region Address Location

  #region Public Properties

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

  public string AdditionalParam { get; set; } = string.Empty;

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

  public ICommand KeyUpAdditionalParam { get; }

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

      ApplyFilterToLocationOfClient();
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

    ApplyFilterToLocationOfClient();
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
    ApplyFilterToLocationOfClient();
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
    ApplyFilterToLocationOfClient();
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
    ApplyFilterToLocationOfClient();
  }

  private void EnterAdditionalParam(object param)
  {
    var additionalParam = AdditionalParam;

    if (IsNameHaveInvalidCharacter(ref additionalParam))
    {
      AdditionalParam = additionalParam;
      return;
    }

    ApplyFilterToLocationOfClient();
  }

  #endregion

  #region Private Properties

  private readonly AddressLocationViewModel _addressLocationViewModel;

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

  //Папки

  #region Folders

  #region Public Properties

  public ObservableCollection<FolderLocationEntityViewModel> FoldersForCreate { get; set; } =
    new ObservableCollection<FolderLocationEntityViewModel>();

  public bool FolderNameUserVersionIsCheck { get; set; } = false;
  public string FolderNameUserVersion { get; set; } = string.Empty;

  #endregion

  #region Event

  public ICommand KeyUpFolderNameUserVersion { get; }

  #endregion

  #region Command Methods

  private void EnterFolderNameUserVersion(object param)
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
  /// </summary>
  /// <exception cref="Exception">
  /// Нет выделенного клиента. Или у выделенного клиента отсутствует путь.
  /// </exception>
  private void CheckLocationForFolders()
  {
    if (SelectedClient.ClientPath == null)
      throw new Exception("Err: " + nameof(CheckLocationForFolders) + " " + nameof(SelectedClient.ClientPath) +
                          "=null");

    var pathForObject = SelectedClient.ClientPath.FullName + Path.DirectorySeparatorChar +
                        ClientEr.FolderObjectsName;

      foreach (var folderForCreate in FoldersForCreate)
      {
        folderForCreate.IsCheck = false;
        folderForCreate.IsEnable = true;

        var dir = pathForObject + Path.DirectorySeparatorChar + SelectedLocation + folderForCreate.FolderDirectory.GetDirectoryPath();
        
        if (Directory.Exists(dir))
        {
          folderForCreate.IsCheck = true;
          folderForCreate.IsEnable = false;
        }
      }
  }

  #endregion

  // Кнопки
  
  #region Buttons

  #region Commands Methods

  public void OnClickButtonCreateDirectory()
  {
    StatusInfo = "Click";

    var clientPath = string.Empty;

    if (SelectedClient == null)
    {
      clientPath = ClientEr.CurrentPath + Path.DirectorySeparatorChar + ClientFilter;
    }
    else
    {
      if (SelectedClient.ClientPath != null) clientPath = SelectedClient.ClientPath.FullName;
    }

    // Создаёт в корневой папке клиента стандартный набор директорий.
    foreach (var directoryInClient in ClientEr.DirectoriesInClient.Folders)
    {
      CreateFolder(clientPath, directoryInClient);
    }

    // Составляет путь директории объекта (локации)
    var locationPath = clientPath + Path.DirectorySeparatorChar + ClientEr.FolderObjectsName;
    var isValidLocation = false;

    if (CityName != string.Empty)
    {
      locationPath += Path.DirectorySeparatorChar + CityName;
      isValidLocation = true;
    }

    if (StreetName != string.Empty)
    {
      locationPath += ", " + StreetName;
    }

    if (HouseNumber != string.Empty)
    {
      locationPath += ", " + HouseNumber;
    }

    if (AdditionalParam != string.Empty)
    {
      if (CityName != string.Empty)
      {
        locationPath += " - ";
      }
      else
      {
        locationPath += Path.DirectorySeparatorChar;
        isValidLocation = true;
      }

      locationPath += AdditionalParam;
    }

    StatusInfo = locationPath;

    // Создаёт в папке объекта (локации) отмеченные директории из стандартного набора.
    if (isValidLocation)
    {
      foreach (var folderForCreate in FoldersForCreate)
      {
        if (folderForCreate.IsCheck)
        {
          CreateFolder(locationPath, folderForCreate.FolderDirectory);
          StatusInfo = folderForCreate.FolderName + " +";
        }
      }

      if (FolderNameUserVersionIsCheck)
      {
        CreateFolder(locationPath, new DirectoryEntity(FolderNameUserVersion));
      }
    }
  }

  /// <summary>
  /// Создаёт дерево директорий относительно указанного пути.
  /// Метод проходит по дереву директорий рекурсивно. Ныряет вглубь.
  /// </summary>
  /// <param name="locationPath">
  /// Путь, в котором будет происходить создание директории
  /// </param>
  /// <param name="directoryEntity">
  /// Директория, которую необходимо создать.
  /// </param>
  private void CreateFolder(string locationPath, DirectoryEntity directoryEntity)
  {
    var dir = locationPath + directoryEntity.GetDirectoryPath();

    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

    if (directoryEntity.ChildDirs != null)
    {
      foreach (var childDir in directoryEntity.ChildDirs)
      {
        CreateFolder(locationPath, childDir);
      }
    }
  }

  #endregion

  #endregion
}