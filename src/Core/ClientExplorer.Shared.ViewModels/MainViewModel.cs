using System;
using System.Collections.ObjectModel;
using System.IO;
using ClientExplorer.Application;

namespace ClientExplorer.Shared.ViewModels;

public class MainViewModel : BaseViewModel
{
  #region Public Properties

  public string StatusInfo { get; set; }
  
  public ObservableCollection<ClientEntityViewModel> Clients { get; set; }

  #endregion

  #region Commands

  #endregion

  #region Events

  #endregion

  #region Constructor

  public MainViewModel()
  {
    Clients = new ObservableCollection<ClientEntityViewModel>();
    //ClientEr.CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
    //TODO: Хардкодим путь до папок с клиентами на время разработки и тестирования. В будущем значение будет вынесено в файл настроек "*.ini". 
    ClientEr.CurrentPath = "/mnt/share/Clients";
    
    StatusInfo = ClientEr.CurrentPath;

    GetClientList();

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
    
    Clients.Clear();
   
    var directoryInfo =new DirectoryInfo(ClientEr.CurrentPath);

    foreach (var directory in directoryInfo.GetDirectories())
    {
      Clients.Add(new ClientEntityViewModel(directory.Name, new DirectoryInfo(directory.FullName)));
    }

    Clients = new ObservableCollection<ClientEntityViewModel>(Clients.OrderBy(i => i.Name));
  }
  
  #endregion
}