using System.IO;

namespace ClientExplorer.Shared.ViewModels;

public sealed class ClientEntityViewModel
{
  public ClientEntityViewModel(string directoryName, DirectoryInfo directoryFullName)
  {
    Name = directoryName;
    Path = directoryFullName;
  }

  public string? Name { get; set; }
  public DirectoryInfo? Path { get; set; }
}