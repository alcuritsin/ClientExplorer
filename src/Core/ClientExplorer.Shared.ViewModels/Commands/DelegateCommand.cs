using System.Windows.Input;

namespace ClientExplorer.Shared.ViewModels;

public class DelegateCommand : ICommand
{
  private readonly Action<object?> _execute;
  private readonly Predicate<object>? _canExecute;

  public DelegateCommand(Action<object?> execute, Predicate<object>? canExecute = null)
  {
    _execute = execute;
    _canExecute = canExecute;
  }

  public bool CanExecute(object? parameter)
  {
    if (_canExecute != null)
    {
      return _canExecute.Invoke(parameter);
    }

    return true;
  }

  public void Execute(object? parameter)
  {
    _execute?.Invoke(parameter);
  }

  public event EventHandler? CanExecuteChanged;
  
  // public void RaiseCanExecuteChanged()
  // {
  //   CanExecuteChanged.Invoke(this, EventArgs.Empty);
  // }
}