using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using static ClientExplorer.Application.ClientExplorerApp;

namespace ClientExplorer.Shared.ViewModels;

public class ClientEntityViewModel : BaseViewModel
{
  public string Name { get; set; }
  public Bitmap Icon { get; private set; }
  public DirectoryInfo ClientPath { get; set; }

  public ClientEntityViewModel(string clientName, DirectoryInfo clientDirectory)
  {
    Name = clientName;
    ClientPath = clientDirectory;
    
    var iconPath = clientDirectory.FullName + Path.DirectorySeparatorChar + Settings.ClientLogoFileName;
    if (File.Exists(iconPath))
    {
      Icon = new Bitmap(iconPath);
    }
    else
    {
      Icon = new Bitmap(AvaloniaLocator.Current.GetService<IAssetLoader>()
        .Open(new Uri($"avares://ClientExplorer/Assets/not-available.png")));
    }
  }
}