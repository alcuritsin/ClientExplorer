using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using ClientExplorer.Application;

namespace ClientExplorer.Shared.ViewModels;

public class MainViewModel : BaseViewModel
{
  #region Public Properties

  //Info панель
  public string StatusInfo { get; set; }

  #region Client

  //Клиент

  public string ClientFilter { get; set; }

  public ObservableCollection<ClientEntityViewModel> SortedClients { get; set; }
  public ClientEntityViewModel SelectedClient { get; set; }

  #endregion

  #region Location of Client

  //Существующие объекты клиента
  public ObservableCollection<string> SortedLocation { get; set; }

  #endregion

  #region Address Location

  public string CityName { get; set; } = string.Empty;
  public string SelectedCity { get; set; } = string.Empty;
  public string StreetName { get; set; } = string.Empty;
  public string HouseNumber { get; set; } = string.Empty;
  public string AdditionalParam { get; set; } = string.Empty;

  public ObservableCollection<string> CitiesFiltered { get; set; } = new ObservableCollection<string>();

  public ObservableCollection<string> StreetsFiltered { get; set; } =
    new ObservableCollection<string>();

  public ObservableCollection<string> HouseNumbersFiltered { get; set; } =
    new ObservableCollection<string>();

  #endregion

  public bool IsCityFocus { get; set; }


  public ICommand OpenClient { get; }
  public ICommand KeyUpClientName { get; }
  public ICommand KeyUpCityName { get; }
  public ICommand LostFocusCityName { get; }
  public ICommand TappedSelectedCity { get; }
  public ICommand KeyUpStreetName { get; }
  

  #endregion

  #region Commands

  private void Open(object param)
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
      if (client.Name.ToUpper().Contains(clientFilter.ToUpper()))
      {
        SortedClients.Add(client);
      }
    }

    StatusInfo += " SortedClients.Count: " + SortedClients.Count;
  }


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


  private void SelectCity(object param)
  {
    CityName = SelectedCity;
    ApplyFilterToCitiesName(null);
    FillStreetsName();
    ApplyFilterToStreetsName(null);
  }
  
  private void FillStreetsName(object param)
  {
    FillStreetsName();
  }
  
  /// <summary>
  /// Применение фильтра к 'Списку Улиц' (StreetsFiltered). На основании (CityName).
  /// </summary>
  /// <param name="param"></param>
  private void ApplyFilterToStreetsName(object param)
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
    
    StatusInfo += " CitiesFiltered.Count: " + CitiesFiltered.Count;
  }

  #endregion

  #region Events

  #endregion

  #region Constructor

  public MainViewModel()
  {
    //ClientEr.CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
    //TODO: Хардкодим путь до папок с клиентами на время разработки и тестирования. В будущем значение будет вынесено в файл настроек "*.ini". 
    ClientEr.CurrentPath = "/mnt/share/Clients";

    OpenClient = new DelegateCommand(Open);
    KeyUpClientName = new DelegateCommand(ApplyFilterToClientsList);
    LostFocusCityName = new DelegateCommand(FillStreetsName);
    TappedSelectedCity = new DelegateCommand(SelectCity);
    
    KeyUpCityName = new DelegateCommand(ApplyFilterToCitiesName);

    KeyUpStreetName = new DelegateCommand(ApplyFilterToStreetsName);

    _addressLocationViewModel = new AddressLocationViewModel();

    SortedClients = new ObservableCollection<ClientEntityViewModel>();

    SortedLocation = new ObservableCollection<string>();


    StatusInfo = ClientEr.CurrentPath;

    GetClientList();
    FillCitiesName();
  }

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

  #region Commands Methods

  #endregion

  #region Private Methods

  private void GetClientList()
  {
    if (ClientEr.CurrentPath == null)
    {
      StatusInfo = "Err: ClientEr.CurrentPath == null";
      return;
    }

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

  private void FillCitiesName()
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

  
  private void FillStreetsName()
  {
    StreetsFiltered.Clear();
    _streetsName.Clear();
    
    foreach (var addressLocation in _addressLocationViewModel.AddressLocations)
    {
      if (Equals(addressLocation.CityName, CityName))
      {
        if (StreetsFiltered.Count == 0)
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

  #endregion

  #region Private Properties

  private List<ClientEntityViewModel> _clientsList = new List<ClientEntityViewModel>();
  private AddressLocationViewModel _addressLocationViewModel;

  private List<string> _citiesName = new List<string>();
  private List<string> _streetsName = new List<string>();
  private List<string> _houseNumbers = new List<string>();

  #endregion
}