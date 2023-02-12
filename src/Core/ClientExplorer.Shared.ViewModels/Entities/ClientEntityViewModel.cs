using System.IO;

namespace ClientExplorer.Shared.ViewModels;

public class ClientEntityViewModel
{
  public string? Name { get; set; }
  public string Icon { get; set; }
  public DirectoryInfo? Path { get; set; }
  public ClientEntityViewModel(string clientName, DirectoryInfo clientDirectory)
  {
    Name = clientName;
    Path = clientDirectory;
    Icon = "path to icon";
  }
}