using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using ClientExplorer.Application;

namespace ClientExplorer.Shared.ViewModels;

public class MainViewModel : BaseViewModel
{
  #region Public Properties

  public string StatusInfo { get; set; }
  
  public ObservableCollection<ClientEntityViewModel> SortedClients { get; set; }
  public ClientEntityViewModel SelectedClient { get; set; }
  
  public ObservableCollection<string> SortedLocation { get; set; }
  
  public ICommand OpenClient { get; }

  #endregion

  #region Commands

  #endregion

  #region Events

  #endregion

  #region Constructor

  public MainViewModel()
  {

    OpenClient = new DelegateCommand(Open);
    
    SortedClients = new ObservableCollection<ClientEntityViewModel>();
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

  private void LoadClientLocation()
  {
    
  }

  #endregion

  #region Commands Methods

  #endregion

  #region Private Methods

  private void GetClientList()
  {
    if (ClientEr.CurrentPath == null)
    {
      StatusInfo = "Err: CurrentPath = null";
      return;
    }
    
    SortedClients.Clear();
   
    var directoryInfo =new DirectoryInfo(ClientEr.CurrentPath);

    foreach (var directory in directoryInfo.GetDirectories())
    {
      SortedClients.Add(new ClientEntityViewModel(directory.Name, new DirectoryInfo(directory.FullName)));
    }

    SortedClients = new ObservableCollection<ClientEntityViewModel>(SortedClients.OrderBy(i => i.Name));
  }
  
  #endregion
}