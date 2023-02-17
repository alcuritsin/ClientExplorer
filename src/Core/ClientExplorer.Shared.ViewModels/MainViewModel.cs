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

  public string StatusInfo { get; set; }

  public string ClientFilter { get; set; }

  public ObservableCollection<ClientEntityViewModel> SortedClients { get; set; }
  public ClientEntityViewModel SelectedClient { get; set; }

  public ObservableCollection<string> SortedLocation { get; set; }

  public bool IsCityFocus { get; set; }


  public ICommand OpenClient { get; }
  public ICommand InputClientFilter { get; }

  #endregion

  #region Commands

  #endregion

  #region Events

  #endregion

  #region Constructor

  public MainViewModel()
  {
    OpenClient = new DelegateCommand(Open);
    InputClientFilter = new DelegateCommand(ApplyFilterToClientsList);

    _clientsList = new List<ClientEntityViewModel>();

    SortedClients = new ObservableCollection<ClientEntityViewModel>();
    SortedLocation = new ObservableCollection<string>();

    //ClientEr.CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
    //TODO: Хардкодим путь до папок с клиентами на время разработки и тестирования. В будущем значение будет вынесено в файл настроек "*.ini". 
    ClientEr.CurrentPath = "/mnt/share/Clients";

    StatusInfo = ClientEr.CurrentPath;

    GetClientList();
  }

  private void Open(object? parametr)
  {
    LoadClientLocation();
  }

  private void ApplyFilterToClientsList(object? parametr)
  {
    StatusInfo = "ClientFilter: " + ClientFilter;

    // Пустой фильтр
    if (Equals(ClientFilter, string.Empty))
    {
      SortedClients.Clear();

      foreach (var client in _clientsList)
      {
        SortedClients.Add(client);
      }

      StatusInfo += " SortedClients.Count: " + SortedClients.Count;

      return;
    }

    if (IsClientNameHaveInvalidCharacter()) return;

    SortedClients.Clear();

    foreach (var client in _clientsList)
    {
      if (client.Name.ToUpper().Contains(ClientFilter.ToUpper()))
      {
        SortedClients.Add(client);
      }
    }

    StatusInfo += " SortedClients.Count: " + SortedClients.Count;
  }

  private bool IsClientNameHaveInvalidCharacter()
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

    if (ClientFilter.Equals(string.Empty)) return false;

    switch (ClientFilter.Last())
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
        if (ClientFilter.Length != 1)
        {
          ClientFilter = ClientFilter.Substring(0, ClientFilter.Length - 1);
        }
        else
        {
          ClientFilter = String.Empty;
        }

        return true;
    }

    return false;
  }


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

  #endregion

  #region Private Properties

  private List<ClientEntityViewModel> _clientsList;

  #endregion
}