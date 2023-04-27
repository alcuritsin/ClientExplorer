using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ClientExplorer.Shared.ViewModels;

namespace ClientExplorer.AvaloniaUI;

public partial class App : Avalonia.Application
{
  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      desktop.MainWindow = new MainWindow
      {
        DataContext = new MainViewModel(),
      };
    }

    base.OnFrameworkInitializationCompleted();
  }
}