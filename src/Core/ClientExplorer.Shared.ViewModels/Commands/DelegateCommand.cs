using System.Windows.Input;

namespace ClientExplorer.Shared.ViewModels;

public class DelegateCommand : ICommand
{
  private readonly Action<object> _execute;
  private readonly Predicate<object>? _canExecute;

  public DelegateCommand(Action<object> execute, Predicate<object>? canExecute = null)
  {
    _execute = execute;
    _canExecute = canExecute;
  }

  public bool CanExecute(object? param)
  {
    if (_canExecute != null)
    {
      return param != null && _canExecute.Invoke(param);
    }

    return true;
  }

  public void Execute(object? param)
  {
    if (param != null) _execute.Invoke(param);
  }

  public event EventHandler? CanExecuteChanged;
  
  // public void RaiseCanExecuteChanged()
  // {
  //   CanExecuteChanged.Invoke(this, EventArgs.Empty);
  // }
}