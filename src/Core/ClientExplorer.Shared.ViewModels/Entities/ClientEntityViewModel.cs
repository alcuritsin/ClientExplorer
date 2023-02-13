using System;
using System.IO;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace ClientExplorer.Shared.ViewModels;

public class ClientEntityViewModel : BaseViewModel
{
  public string? Name { get; set; }
  public Bitmap IconPath { get; private set; }
  public DirectoryInfo? ClientPath { get; set; }
  public ClientEntityViewModel(string clientName, DirectoryInfo clientDirectory)
  {
    Name = clientName;
    ClientPath = clientDirectory;
    IconPath = new Bitmap(AvaloniaLocator.Current.GetService<IAssetLoader>()
      .Open(new Uri($"avares://ClientExplorer/Assets/not-available.png")));
  }
}