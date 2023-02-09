using System.Collections.ObjectModel;
using ClientExplorer.Application;

namespace ClientExplorer.Shared.ViewModels;

public class MainViewModel : BaseViewModel
{
  #region Public Properties

  public string Greeting { get; set; }

  #endregion

  #region Commands

  #endregion

  #region Events

  #endregion

  #region Constructor

  public MainViewModel()
  {
    Greeting = AppDomain.CurrentDomain.BaseDirectory;
  }

  #endregion

  #region Commands Methods

  #endregion

  #region Private Methods

  #endregion
}