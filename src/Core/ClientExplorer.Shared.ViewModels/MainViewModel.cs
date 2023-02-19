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

    public string AddressLocationCityFilter { get; set; }
    public string AddressLocationFilterStreet { get; set; }
    public string AddressLocationFilterHouseNumber { get; set; }
    public string AdditionalParam { get; set; }

    public ObservableCollection<string> SortedAddressLocationByCity { get; set; } = new ObservableCollection<string>();

    public ObservableCollection<string> SortedAddressLocationByStreet { get; set; } =
        new ObservableCollection<string>();

    public ObservableCollection<string> SortedAddressLocationByHouseNumber { get; set; } =
        new ObservableCollection<string>();

    #endregion

    public bool IsCityFocus { get; set; }


    public ICommand OpenClient { get; }
    public ICommand InputClientFilter { get; }
    public ICommand InputCityFilter { get; }

    #endregion

    #region Commands

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
        InputClientFilter = new DelegateCommand(ApplyFilterToClientsList);
        InputCityFilter = new DelegateCommand(ApplyFilterToCityName);

        _addressLocationViewModel = new AddressLocationViewModel();

        SortedClients = new ObservableCollection<ClientEntityViewModel>();

        SortedLocation = new ObservableCollection<string>();


        StatusInfo = ClientEr.CurrentPath;

        GetClientList();
        GetCitiesName();
    }


    private void Open(object? parametr)
    {
        LoadClientLocation();
    }


    private void ApplyFilterToCityName(object? parametr)
    {
        var cityFilter = AddressLocationCityFilter;
        if (Equals(cityFilter, string.Empty))
        {
            SortedAddressLocationByCity.Clear();

            foreach (var cityName in _citiesName)
            {
                SortedAddressLocationByCity.Add(cityName);
            }
            
            return;
        }

        if (IsNameHaveInvalidCharacter(ref cityFilter))
        {
            AddressLocationCityFilter = cityFilter;
            return;
        }
        
        SortedAddressLocationByCity.Clear();

        foreach (var cityName in _citiesName)
        {
            if (cityName.ToUpper().Contains(cityFilter.ToUpper()))
            {
                SortedAddressLocationByCity.Add(cityName);
            }
        }
        
    }

    private void ApplyFilterToClientsList(object? parametr)
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
    /// Удаляет из имени 'Клиента', запрещённые символы для именования папок в операцинной системе. 
    /// </summary>
    /// <returns>
    /// true - был найден запрещённый символ
    /// false - запрещённых символов не обнаружено
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
            SortedClients.Add(new ClientEntityViewModel(directory.Name, new DirectoryInfo(directory.FullName)));
        }

        SortedClients = new ObservableCollection<ClientEntityViewModel>(SortedClients.OrderBy(i => i.Name));

        _clientsList = SortedClients.ToList();
    }

    private void GetCitiesName()
    {
        foreach (var addressLocation in _addressLocationViewModel.AddressLocations)
        {
            if (SortedAddressLocationByCity.Count == 0)
            {
                _citiesName.Add(addressLocation.CityName);
                SortedAddressLocationByCity.Add(addressLocation.CityName);
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
                SortedAddressLocationByCity.Add(addressLocation.CityName);
            }
        }
    }

    #endregion

    #region Private Properties

    private List<ClientEntityViewModel> _clientsList = new List<ClientEntityViewModel>();
    private AddressLocationViewModel _addressLocationViewModel;

    private List<string> _citiesName = new List<string>();

    #endregion
}